using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector.Network.Server
{
    internal class TcpServer
    {
        private readonly ILogger log = Log.ForContext<TcpServer>();

        private readonly IPEndPoint ipEndPoint;
        private TcpListener? listener;
        private bool running;
        private readonly List<AcceptedClient> clients = [];
        private readonly CancellationTokenSource cts;
        private readonly ServerProgressUpdater serverProgressUpdater;

        public TcpServer(int port, ServerProgressUpdater serverProgressUpdater)
        {
            ipEndPoint = new(IPAddress.Any, port);
            cts = new CancellationTokenSource();
            this.serverProgressUpdater = serverProgressUpdater;
            string logMessage = $"TCP Server initialized on port {port}";
            log.Information(logMessage);
            this.serverProgressUpdater.UpdateLog(logMessage);
        }

        public void Stop()
        {
            running = false;
            cts.Cancel();
        }

        public async Task Start(string connectionString)
        {
            await Run(connectionString);
        }

        private async Task Run(string connectionString)
        {
            listener = new(ipEndPoint);
            listener.Start();
            running = true;
            while (running)
            {
                CancellationTokenSource clientCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                try
                {
                    serverProgressUpdater.UpdateLog("Waiting for incoming TCP client connection...");
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
                    Socket socket = tcpClient.Client;
                    /*
                    uint on = 1;
                    uint keepAliveTime = 30000; // 30 seconds
                    uint keepAliveInterval = 1000; // 1 second
                    byte[] inValue = new byte[12];
                    BitConverter.GetBytes(on).CopyTo(inValue, 0);
                    BitConverter.GetBytes(keepAliveTime).CopyTo(inValue, 4);
                    BitConverter.GetBytes(keepAliveInterval).CopyTo(inValue, 8);
                    */
                    // 30 seconds idle time and 1 second interval
                    socket.IOControl(IOControlCode.KeepAliveValues, [1, 0, 0, 0, 0xE8, 0x03, 0x00, 0x00, 0xE8, 0x03, 0x00, 0x00], null);

                    AcceptedClient client = new(tcpClient, clientCancellationTokenSource, serverProgressUpdater);
                    IPEndPoint? remoteIpEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                    string logMessage = $"New client with IP {remoteIpEndPoint?.Address} connected";
                    log.Information(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    clients.Add(client);
                    Task clientTask = client.Run(connectionString); //don't await
                    clientTask.ContinueWith(t => clients.Remove(client));
                }
                catch (OperationCanceledException)
                {
                    log.Information("Stopping of TCP server requested");
                    listener.Stop();
                    listener.Dispose();
                    break;
                }
                catch (Exception ex)
                {
                    string logMessage = $"Error accepting TCP client";
                    log.Error(ex, logMessage);
                    serverProgressUpdater.UpdateLog($"{logMessage}: {ex.Message}", true);
                    listener.Stop();
                    listener.Dispose();
                    throw;
                }
            }
        }
    }

    internal class AcceptedClient(TcpClient client, CancellationTokenSource clientCancellationTokenSource, ServerProgressUpdater serverProgressUpdater)
    {
        private readonly ILogger log = Log.ForContext<AcceptedClient>();

        private readonly NetworkStream stream = client.GetStream();
        private readonly string clientIPAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

        public async Task Run(string dbConnectionString)
        {
            DbRepository dbRepository = new(dbConnectionString);
            StreamReader r = new(stream);
            StreamWriter w = new(stream)
            {
                AutoFlush = true
            };

            while (true)
            {
                string? qsoMessageJson = await r.ReadLineAsync(clientCancellationTokenSource.Token);
                if (qsoMessageJson == null) continue;
                QsoMessage? qsoMessage = JsonSerializer.Deserialize<QsoMessage>(qsoMessageJson);
                if (qsoMessage == null) continue;

                ServerResponse response;
                List<Dictionary<string, string>> qsoRecords = [];
                try
                {
                    if (!qsoMessage.IsTest)
                    {
                        qsoRecords = ParseQsoMessage(qsoMessage);
                        if (qsoRecords.Count == 0)
                        {
                            log.Warning("No valid QSO records found in the message from client {ClientIP}, source {Source}, format {Format}, data {Data}", 
                                clientIPAddress, 
                                qsoMessage.Source,
                                qsoMessage.OriginalFormat,
                                qsoMessage.OriginalQsoData
                                );
                            throw new ArgumentException("No valid QSO records found in ADIF data");
                        }

                        dbRepository.SaveQsoRecords(qsoRecords, isTemporary: false);
                        serverProgressUpdater.UpdateLog($"{qsoMessage}", true);
                        foreach (var rec in qsoRecords)
                        {
                            string qsoInfo = $"{rec["SOURCE_IP_ADDRESS"]} {rec["SOURCE_NAME"]} {rec["QSO_TIME"]} {rec["BAND"]} {rec["FREQ"]} {rec["MODE"]} {rec["CALL"]}";
                            log.Information("Saved QSO: {qsoInfo}", qsoInfo);
                            serverProgressUpdater.UpdateLog(qsoInfo);
                            serverProgressUpdater.UpdateProgress(ServerProgressUpdater.ParseDateTime(rec["QSO_TIME"]), rec["MODE"], null);
                        }
                    }
                    response = new ServerResponse(ServerResponseStatus.Ok);
                }
                catch (SqliteException ex)
                {
                    log.Error("SQLite error while processing message from client {ClientIP}: {ErrorMessage}", clientIPAddress, ex.Message);
                    response = new ServerResponse(ServerResponseStatus.SqliteError, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    log.Error("Argument error while processing message from client {ClientIP}: {ErrorMessage}", clientIPAddress, ex.Message);
                    response = new ServerResponse(ServerResponseStatus.ArgumentError, ex.Message);
                }
                catch (OperationCanceledException)
                {
                    log.Information("Client {ClientIP} disconnected caused by Server stopping requested", clientIPAddress);
                    clientCancellationTokenSource.Dispose();
                    throw;
                }
                catch (Exception ex)
                {
                    log.Error("Unknown error while processing message from client {ClientIP}: {ErrorMessage}", clientIPAddress, ex.Message);
                    response = new ServerResponse(ServerResponseStatus.UnknownError, ex.Message);
                }

                if (qsoMessage.IsTest)
                {
                    string message = $"Got server status request from {clientIPAddress}. Status: {response.Status}";
                    log.Debug(message);
                    serverProgressUpdater.UpdateLog(message, true);
                }

                await w.WriteLineAsync(JsonSerializer.Serialize(response));
                await w.FlushAsync();
            }
        }

        private List<Dictionary<string, string>> ParseQsoMessage(QsoMessage qsoMessage)
        {
            List<Dictionary<string, string>> qsoRecords = [];
            try
            {
                qsoRecords = qsoMessage.OriginalFormat switch
                {
                    "ADIF" => AdifToTableFieldsMapper.Map(qsoMessage, sourceIpAddress: clientIPAddress),
                    "N1MM" => N1mmContactInfoToTableFieldsMapper.Map(qsoMessage, clientIPAddress),
                    _ => throw new ArgumentException($"Unsupported message format: {qsoMessage.OriginalFormat}"),
                };

                if (serverProgressUpdater.IsDebug)
                {
                    foreach (var record in qsoRecords)
                    {
                        StringBuilder logMessage = new();
                        foreach (var kv in record)
                        {
                            logMessage.AppendLine(kv.Key + ": " + kv.Value);
                        }
                        logMessage.AppendLine("----");
                        log.Debug("Parsed QSO record from client {ClientIP}:\n{Record}", clientIPAddress, logMessage);
                        serverProgressUpdater.UpdateLog(logMessage.ToString(), true);
                    }
                    serverProgressUpdater.UpdateLog("----", true);
                }
            }
            catch (Exception ex)
            {
                // Handle parsing errors
                string logMessage = "ADIF parsing error";
                log.Error(ex, logMessage);
                serverProgressUpdater.UpdateLog($"{logMessage}: {ex.Message}", true);
                throw new ArgumentException(logMessage, ex);
            }
            return qsoRecords;
        }
    }
}
