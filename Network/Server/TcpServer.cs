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
        private readonly IDbRepository dbRepository;

        public TcpServer(int port, ServerProgressUpdater serverProgressUpdater, IDbRepository dbRepository)
        {
            ipEndPoint = new(IPAddress.Any, port);
            cts = new CancellationTokenSource();
            this.serverProgressUpdater = serverProgressUpdater;
            this.dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
            string logMessage = $"TCP Server initialized on port {port}";
            log.Information(logMessage);
            this.serverProgressUpdater.UpdateLog(logMessage);
        }

        public void Stop()
        {
            running = false;
            cts.Cancel();
        }

        public async Task Start()
        {
            listener = new(ipEndPoint);
            listener.Start();
            running = true;
            log.Information("Server started on port {port}", ipEndPoint.Port);
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

                    AcceptedClient client = new(tcpClient, clientCancellationTokenSource, serverProgressUpdater, dbRepository);
                    IPEndPoint? remoteIpEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                    string logMessage = $"New client with IP {remoteIpEndPoint?.Address} connected";
                    log.Information(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    clients.Add(client);
                    Task clientTask = client.Run(); //don't await
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

    internal class AcceptedClient(TcpClient client, CancellationTokenSource clientCancellationTokenSource, ServerProgressUpdater serverProgressUpdater, IDbRepository dbRepository)
    {
        private readonly ILogger log = Log.ForContext<AcceptedClient>();

        private readonly TcpClient client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly CancellationTokenSource clientCancellationTokenSource = clientCancellationTokenSource ?? throw new ArgumentNullException(nameof(clientCancellationTokenSource));
        private readonly ServerProgressUpdater serverProgressUpdater = serverProgressUpdater ?? throw new ArgumentNullException(nameof(serverProgressUpdater));
        private readonly NetworkStream stream = client.GetStream();
        private readonly string clientIPAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        private readonly IDbRepository dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));

        public async Task Run()
        {
            StreamReader r = new(stream);
            StreamWriter w = new(stream)
            {
                AutoFlush = true
            };

            while (true)
            {
                ServerResponse response;
                QsoMessage? qsoMessage;
                try
                {
                    log.Debug("Waiting for message from client {ClientIP}...", clientIPAddress);
                    string? qsoMessageJson = await r.ReadLineAsync(clientCancellationTokenSource.Token);
                    if (qsoMessageJson == null)
                    {
                        string logMessage = $"Client {clientIPAddress} disconnected (null message received). Closing client connection";
                        log.Warning(logMessage);
                        throw new SocketException((int)SocketError.ConnectionAborted, logMessage);
                    }

                    qsoMessage = JsonSerializer.Deserialize<QsoMessage>(qsoMessageJson);
                    if (qsoMessage == null)
                    {
                        log.Warning("Got null qsoMessage from Client {ClientIP} after deserialization. Ignoring", clientIPAddress);
                        continue;
                    }

                    if (qsoMessage.IsHeartBeat) {
                        string message = $"Got heartbeat from {clientIPAddress}";
                        log.Debug(message);
                        serverProgressUpdater.UpdateLog(message, true);
                    } 
                    else
                    {
                        ProcessQsoMessage(qsoMessage, dbRepository);
                    }

                    response = new(ServerResponseStatus.Ok);
                }
                catch (SqliteException ex)
                {
                    log.Error("SQLite error while processing message from client {ClientIP}: {ErrorMessage}", clientIPAddress, ex.Message);
                    response = new (ServerResponseStatus.SqliteError, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    log.Error("Argument error while processing message from client {ClientIP}: {ErrorMessage}", clientIPAddress, ex.Message);
                    response = new (ServerResponseStatus.ArgumentError, ex.Message);
                }
                catch (OperationCanceledException)
                {
                    log.Information("Client {ClientIP} disconnected caused by Server stopping requested. Closing client connection", clientIPAddress);
                    serverProgressUpdater.UpdateLog($"Client {clientIPAddress} disconnected caused by Server stopping requested. Closing client connection", true);
                    clientCancellationTokenSource.Dispose();
                    client.Close();
                    client.Dispose();
                    throw;
                }
                catch (SocketException ex)
                {
                    string logMessage = $"Client {clientIPAddress} socket exception while reading request from Client. ErrorCode {ex.ErrorCode}, native ErrorCode {ex.NativeErrorCode}. Closing client connection";
                    log.Warning(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    client.Close();
                    client.Dispose();
                    break;
                }
                catch (Exception ex)
                {
                    string logMessage = $"Unknown error while processing message from client {clientIPAddress}: {ex.Message}";
                    log.Error(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    response = new ServerResponse(ServerResponseStatus.UnknownError, ex.Message);
                }

                log.Debug("Sending response to client {ClientIP}: {Response}", clientIPAddress, response);

                try
                {
                    await w.WriteLineAsync(JsonSerializer.Serialize(response));
                    await w.FlushAsync();
                }
                catch (SocketException ex) 
                {
                    string logMessage = $"Client {clientIPAddress} socket exception while sending response to Client. ErrorCode {ex.ErrorCode}, native ErrorCode {ex.NativeErrorCode}. Closing client connection";
                    log.Warning(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    client.Close();
                    client.Dispose();
                    break;
                }
                catch (Exception ex)
                {
                    string logMessage = $"Unknown error while while sending response to client {clientIPAddress}: {ex.Message}";
                    log.Warning(logMessage);
                    serverProgressUpdater.UpdateLog(logMessage);
                    client.Close();
                    client.Dispose();
                    break;
                }
            }
        }

        private void ProcessQsoMessage(QsoMessage qsoMessage, IDbRepository dbRepository) {
            List<Dictionary<string, string>> qsoRecords = ParseQsoMessage(qsoMessage);
            log.Debug("Parsed {RecordCount} QSO records from message from client {ClientIP}, source {Source}, format {Format}",
                qsoRecords.Count,
                clientIPAddress,
                qsoMessage.Source,
                qsoMessage.OriginalFormat
                );
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
            log.Debug("Saving {RecordCount} QSO records from message from client {ClientIP} to database", qsoRecords.Count, clientIPAddress);
            dbRepository.SaveQsoRecords(qsoRecords, isTemporary: false);
            serverProgressUpdater.UpdateLog($"{qsoMessage}", true);
            foreach (var rec in qsoRecords)
            {
                string isReplace = qsoMessage.Replace ? "Correction" : "";
                string qsoInfo = $"{rec["SOURCE_IP_ADDRESS"]} {rec["SOURCE_NAME"]} {rec["QSO_TIME"]} {rec["BAND"]} {rec["FREQ"]} {rec["MODE"]} {rec["CALL"]} {isReplace}";
                log.Information("Saved QSO: {qsoInfo}", qsoInfo);
                serverProgressUpdater.UpdateLog(qsoInfo);
                serverProgressUpdater.UpdateProgress(ServerProgressUpdater.ParseDateTime(rec["QSO_TIME"]), rec["MODE"], null);
            }
        }

        private List<Dictionary<string, string>> ParseQsoMessage(QsoMessage qsoMessage)
        {
            List<Dictionary<string, string>> qsoRecords = [];
            try
            {
                log.Debug("Parsing QSO message from client {ClientIP}, source {Source}, format {Format}", clientIPAddress, qsoMessage.Source, qsoMessage.OriginalFormat);
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
