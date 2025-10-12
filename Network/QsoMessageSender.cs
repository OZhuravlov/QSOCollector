using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector.Network
{
    public class QsoMessageSender
    {
        private const int responseTimeoutMs = 5000;

        private readonly string serverIp;
        private readonly int serverPort;
        private readonly BlockingCollection<QsoMessage> qsoMessageQueue;
        private readonly DbRepository dbRepository;
        private readonly ClientProgressUpdater progressUpdater;
        private readonly CancellationTokenSource cancellationTokenSource;
        private TcpClientInstance? tcpClient;

        public QsoMessageSender(string serverIp, int serverPort, BlockingCollection<QsoMessage> qsoMessageQueue,
            DbRepository dbRepository, ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.qsoMessageQueue = qsoMessageQueue;
            this.dbRepository = dbRepository;
            this.progressUpdater = progressUpdater;
            this.cancellationTokenSource = cancellationTokenSource;
            try
            {
                tcpClient = new TcpClientInstance(serverIp, serverPort, progressUpdater);
                progressUpdater.UpdateServerStatus("Active", null);
            }
            catch (SocketException ex)
            {
                tcpClient = null;
                progressUpdater.UpdateServerStatus("Unavailable", $"Unexpected error while creating QSO sender: {ex.Message}");
            }

        }

        public bool IsConnected()
        {
            bool isConnected = tcpClient != null && tcpClient.IsConnected();
            if (!isConnected)
            {
                progressUpdater.UpdateServerStatus("Unavailable", "Not connected to server");
            }
            return isConnected;
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
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            progressUpdater.UpdateLog("Qso Message handler has been stopped");
                            return;
                        }
                        await SendToServer(new() { Source = "TEST" });
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
                        await tcpClient.SendMessage(qsoMessageJson, responseTimeoutMs, qsoMessage.Source, qsoMessage.IsTest);
                        retry = false;
                    }
                    catch (Exception)
                    {
                        if (i-- > 0)
                        {
                            retry = true;
                            tcpClient = new(serverIp, serverPort, progressUpdater);
                            await Task.Delay(TimeSpan.FromSeconds(1));
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

        private List<Dictionary<string, string>> ParseMessage(QsoMessage qsoMessage)
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