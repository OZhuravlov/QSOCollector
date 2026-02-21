using Microsoft.Data.Sqlite;
using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector.Network.Server
{
    internal class TcpServer
    {
        public const int AcceptedTcpClientReadTimeoutSeconds = 30;

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
            this.serverProgressUpdater.UpdateLog($"TCP Server initialized on port {port}");
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
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    AcceptedClient client = new(tcpClient, clientCancellationTokenSource, serverProgressUpdater);
                    IPEndPoint? remoteIpEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                    serverProgressUpdater.UpdateLog($"New client with IP {remoteIpEndPoint?.Address} connected");
                    clients.Add(client);
                    Task clientTask = client.Run(connectionString); //don't await
                    clientTask.ContinueWith(t => clients.Remove(client));
                }
                catch (OperationCanceledException)
                {
                    listener.Stop();
                    listener.Dispose();
                    break;
                }
                catch (Exception e)
                {
                    serverProgressUpdater.UpdateLog($"Error accepting TCP client: {e.Message}", true);
                    listener.Stop();
                    listener.Dispose();
                    throw;
                }
            }
        }
    }

    internal class AcceptedClient(TcpClient client, CancellationTokenSource clientCancellationTokenSource, ServerProgressUpdater serverProgressUpdater)
    {
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

            try
            {
                while (!clientCancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Wait for a line from the client with both a timeout and the ability to cancel
                    // using the linked client cancellation token. WaitAsync will throw a TimeoutException
                    // on timeout or OperationCanceledException when the token is cancelled.
                    string? qsoMessageJson = await r.ReadLineAsync()
                        .WaitAsync(TimeSpan.FromSeconds(TcpServer.AcceptedTcpClientReadTimeoutSeconds), clientCancellationTokenSource.Token);
                    if (qsoMessageJson == null) break;

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
                                throw new ArgumentException("No valid QSO records found in ADIF data");
                            }

                            dbRepository.SaveQsoRecords(qsoRecords, isTemporary: false);
                            serverProgressUpdater.UpdateLog($"{qsoMessage}", true);
                            foreach (var item in qsoRecords)
                            {
                                serverProgressUpdater.UpdateProgress(ServerProgressUpdater.ParseDateTime(item["QSO_TIME"]), item["MODE"], null);
                            }
                        }
                        response = new ServerResponse(ServerResponseStatus.Ok);
                    }
                    catch (SqliteException ex)
                    {
                        response = new ServerResponse(ServerResponseStatus.SqliteError, ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        response = new ServerResponse(ServerResponseStatus.ArgumentError, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        response = new ServerResponse(ServerResponseStatus.UnknownError, ex.Message);
                    }

                    if (qsoMessage.IsTest)
                    {
                        string message = $"Got server status request from {clientIPAddress}. Status: {response.Status}";
                        serverProgressUpdater.UpdateLog(message, true);
                    }
                    else
                    {
                        foreach (var rec in qsoRecords)
                        {
                            string message = $"{rec["SOURCE_IP_ADDRESS"]} {rec["SOURCE_NAME"]} {rec["QSO_TIME"]} {rec["BAND"]} {rec["FREQ"]} {rec["MODE"]} {rec["CALL"]}";
                            serverProgressUpdater.UpdateLog(message);
                        }
                    }
                    await w.WriteLineAsync(JsonSerializer.Serialize(response));
                    await w.FlushAsync();
                }
            }
            catch (TimeoutException)
            {
                serverProgressUpdater.UpdateLog($"Client {clientIPAddress} disconnected by timeout");
                client.Close();
            }
            catch (OperationCanceledException)
            {
                clientCancellationTokenSource.Dispose();
                throw;
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
                    // Log parsed records
                    foreach (var record in qsoRecords)
                    {
                        StringBuilder logMessage = new();
                        foreach (var kv in record)
                        {
                            logMessage.AppendLine(kv.Key + ": " + kv.Value);
                        }
                        logMessage.AppendLine("----");
                        serverProgressUpdater.UpdateLog(logMessage.ToString(), true);
                    }
                    serverProgressUpdater.UpdateLog("----", true);
                }
            }
            catch (Exception ex)
            {
                // Handle parsing errors
                serverProgressUpdater.UpdateLog($"ADIF parsing error: {ex.Message}", true);
                throw new ArgumentException("ADIF parsing error", ex);
            }
            return qsoRecords;
        }
    }
}
