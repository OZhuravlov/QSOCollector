using QSOCollector.Data;
using QSOCollector.Helpers;
using QSOCollector.Models;
using QSOCollector.Parsers;
using Serilog;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector.Network.Client
{
    public class QsoMessageSender
    {
        private const int responseTimeoutMs = 5000;
        private const int maxRetryCount = 3;

        private readonly ILogger log = Log.ForContext<QsoMessageSender>();

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
                log.Information("Connected to server at {ServerIp}:{ServerPort}", serverIp, serverPort);
                progressUpdater.UpdateServerStatus("Active", null);
            }
            catch (SocketException ex)
            {
                tcpClient = null;
                log.Error(ex, "Failed to connect to server at {ServerIp}:{ServerPort}", serverIp, serverPort);
                progressUpdater.UpdateServerStatus("Unavailable", $"Unexpected error while creating QSO sender: {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            bool isConnected = tcpClient != null && tcpClient.IsConnected();
            if (!isConnected)
            {
                log.Warning("Not connected to server at {ServerIp}:{ServerPort}", serverIp, serverPort);
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
                            string logMessage = "Qso Message handler has been stopped by request";
                            log.Information(logMessage);
                            progressUpdater.UpdateLog(logMessage);
                            return;
                        }
                        await SendToServer(new() { Source = "TEST" });
                        continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    string logMessage = "Qso Message handler has been stopped";
                    log.Warning(logMessage);
                    progressUpdater.UpdateLog(logMessage);
                    return;
                }

                try
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        string logMessage = "Client to be stopped requested";
                        log.Warning(logMessage);
                        HandleServerConnectionError(qsoMessage, logMessage);
                    }
                    else
                    {
                        dbRepository.SaveRawQso(qsoMessage);
                        await SendToServer(qsoMessage);
                    }
                }
                catch (ArgumentException ex)
                {
                    string logMessage = $"Incorrect {qsoMessage.OriginalFormat} format, source {qsoMessage.Source}, data: {qsoMessage.OriginalQsoData}\r\n: {ex}\r\nIgnored";
                    log.Error(logMessage);
                    progressUpdater.UpdateProgress(false, false, false, true, logMessage);
                    continue;
                }
            }
        }

        private async Task SendToServer(QsoMessage qsoMessage)
        {
            try
            {
                bool shouldRetry = false;
                int tryNumber = 0;
                do
                {
                    try
                    {
                        if (qsoMessage.IsTest)
                        {
                            log.Debug("Try to send heartbeat message to server");
                        }
                        else {
                            log.Information("Try to send QSO message to server: source {Source}, format {Format}", qsoMessage.Source, qsoMessage.OriginalFormat);
                        }
                        
                        await tcpClient.SendMessage(qsoMessage, responseTimeoutMs);
                        shouldRetry = false;
                    }
                    catch (Exception)
                    {
                        log.Warning("Failed to send message to server at {ServerIp}:{ServerPort}", serverIp, serverPort);
                        if (++tryNumber > maxRetryCount) {
                            throw;
                        }
                        log.Warning("Recreate TCP client and retry #{tryNumber}", tryNumber);
                        shouldRetry = true;
                        tcpClient = new(serverIp, serverPort, progressUpdater);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;
                    }
                } while (shouldRetry);
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
            log.Warning(logMessage);
            progressUpdater.UpdateServerStatus("Unavailable", logMessage);

            if (qsoMessage.IsTest) return;

            log.Information("Save QSO message to local DB due to server connection error: source {Source}, format {Format}", qsoMessage.Source, qsoMessage.OriginalFormat);
            log.Debug("QSO message data: {QsoMessage}", qsoMessage.OriginalQsoData);
            
            progressUpdater.UpdateLog($"The following QSO messages will be temporarely saved to local DB:\n{qsoMessage}", true);

            List<Dictionary<string, string>> qsoRecords = ParseMessage(qsoMessage);
            dbRepository.SaveQsoRecords(qsoRecords, isTemporary: true);
            progressUpdater.UpdateTempSaved(qsoRecords.Count);

            string savedQsoLogMessage = $"{qsoRecords.Count} QSOs from {qsoMessage.Source} saved to local Database";
            log.Information(savedQsoLogMessage);
            progressUpdater.UpdateLog(savedQsoLogMessage);
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
                    _ => HandleUsupportedFormat(qsoMessage.OriginalFormat),
                };
            }
            catch (Exception ex)
            {
                string logMessage = "QSO message parsing error";
                log.Error(ex, "{Message}: source = {Source}, format = {Format}", logMessage, qsoMessage.Source, qsoMessage.OriginalFormat);
                log.Debug("QSO message data: {QsoMessage}", qsoMessage.OriginalQsoData);
                throw new ArgumentException(logMessage, ex);
            }
            return qsoRecords;
        }

        private List<Dictionary<string, string>> HandleUsupportedFormat(string format)
        {
            string logMessage = $"Unsupported message format: {format}";
            log.Warning(logMessage);
            throw new ArgumentException(logMessage);
        }
    }
}