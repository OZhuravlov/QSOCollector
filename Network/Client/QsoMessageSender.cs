using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector.Network.Client
{
    public class QsoMessageSender
    {
        // timeouts are configurable via environment variables (see TimeoutSettings)

        private readonly string serverIp;
        private readonly int serverPort;
        private readonly BlockingCollection<QsoMessage> qsoMessageQueue;
        private readonly DbRepository dbRepository;
        private readonly ClientProgressUpdater progressUpdater;
        private readonly CancellationTokenSource cancellationTokenSource;
        private TcpClientInstance? tcpClient = null;
        private readonly TimeoutOptions timeoutOptions;

        public QsoMessageSender(string serverIp, int serverPort, BlockingCollection<QsoMessage> qsoMessageQueue,
            DbRepository dbRepository, ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource, TimeoutOptions timeoutOptions)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.qsoMessageQueue = qsoMessageQueue;
            this.dbRepository = dbRepository;
            this.progressUpdater = progressUpdater;
            this.cancellationTokenSource = cancellationTokenSource;
            this.timeoutOptions = timeoutOptions ?? new TimeoutOptions();
        }

        /// <summary>
        /// Ensure there's an active connection to the server. Attempts to connect if not already connected.
        /// Returns true when connected, false otherwise.
        /// </summary>
        public async Task<bool> EnsureConnectedAsync()
        {
            try
            {
                if (tcpClient == null || !tcpClient.IsConnected()) {
                    tcpClient = await TcpClientInstance.CreateAsync(serverIp, serverPort, progressUpdater, timeoutOptions.ConnectTimeoutMs);
                }
                progressUpdater.UpdateServerStatus("Active", null);
                return true;
            }
            catch (Exception ex)
            {
                try { tcpClient?.Terminate(); } catch { }
                tcpClient = null;
                progressUpdater.UpdateServerStatus("Unavailable", $"Cannot connect to server: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(1));
                return false;
            }
        }

        public async Task Start()
        {
            while (true)
            {
                QsoMessage qsoMessage;
                try
                {
                    if (!qsoMessageQueue.TryTake(out qsoMessage, 5000))
                    {
                        await SendHeartBeat();
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    progressUpdater.UpdateLog("Qso Message handler has been stopped");
                    return;
                }

                try
                {
                    dbRepository.SaveRawQso(qsoMessage);
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        HandleServerConnectionError(qsoMessage, "Client to be stopped requested");
                    }
                    else
                    {
                        await SendToServer(qsoMessage);
                    }
                }
                catch (ArgumentException ex)
                {
                    progressUpdater.UpdateProgress(false, false, false, true,
                        $"Incorrect {qsoMessage.OriginalFormat} format, source {qsoMessage.Source}, data: {qsoMessage.OriginalQsoData}\r\n: {ex}\r\nIgnored");
                    continue;
                }
            }
        }

        private async Task SendHeartBeat()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                progressUpdater.UpdateLog("Qso Message handler has been stopped");
                return;
            }
            await SendToServer(new() { Source = "TEST" });
        }

        private async Task SendToServer(QsoMessage qsoMessage)
        {
            string qsoMessageJson = JsonSerializer.Serialize(qsoMessage);
            try
            {
                bool retry = false;
                int i = 3;
                do
                {
                    try
                    {
                        bool isConnected = await EnsureConnectedAsync();
                        if (!isConnected) throw new Exception("Server is not available");
                        await tcpClient.SendMessage(qsoMessageJson, timeoutOptions.WriteTimeoutMs, timeoutOptions.ResponseTimeoutMs, qsoMessage.Source, qsoMessage.IsTest);
                        retry = false;
                    }
                    catch (Exception)
                    {
                        if (i-- > 0)
                        {
                            retry = true;
                            continue;
                        }
                        throw;
                    }
                } while (retry);
            }
            catch (OperationCanceledException)
            {
                HandleServerConnectionError(qsoMessage, "Connection to Server has been cancelled");
            }
            catch (SocketException)
            {
                HandleServerConnectionError(qsoMessage, "Looks like server is not available");
            }
        }

        private void HandleServerConnectionError(QsoMessage qsoMessage, string logMessage)
        {
            progressUpdater.UpdateServerStatus("Unavailable", logMessage);
            if (qsoMessage.IsTest) return;
            progressUpdater.UpdateLog($"The following QSO messages will be temporarely saved to local DB:\n{qsoMessage}", true);
            List<Dictionary<string, string>> qsoRecords = ParseMessage(qsoMessage);
            dbRepository.SaveQsoRecords(qsoRecords, isTemporary: true);
            progressUpdater.UpdateTempSaved(qsoRecords.Count);
            progressUpdater.UpdateLog($"{qsoRecords.Count} QSOs from {qsoMessage.Source} saved to local Database");
        }

        private static List<Dictionary<string, string>> ParseMessage(QsoMessage qsoMessage)
        {
            List<Dictionary<string, string>> qsoRecords;
            try
            {
                string sourceIpAddress = IPAddress.Loopback.ToString();
                qsoRecords = qsoMessage.OriginalFormat switch
                {
                    "ADIF" => AdifToTableFieldsMapper.Map(qsoMessage, sourceIpAddress: sourceIpAddress),
                    "N1MM" => N1mmContactInfoToTableFieldsMapper.Map(qsoMessage, sourceIpAddress),
                    _ => throw new ArgumentException($"Unsupported message format: {qsoMessage.OriginalFormat}"),
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException("QSO message parsing error", ex);
            }
            return qsoRecords;
        }
    }
}