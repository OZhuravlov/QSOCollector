using QSOCollector.Helpers;
using QSOCollector.Models;
using Serilog;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace QSOCollector.Network.Client
{
    public class UdpClientListener
    {
        private readonly ILogger log = Log.ForContext<UdpClientListener>();

        private readonly ListenerConfig listenerConfig;
        private readonly UdpClient? forwardUdpClient;
        private readonly BlockingCollection<QsoMessage> qsoMessageQueue;
        private readonly ClientProgressUpdater progressUpdater;
        private readonly CancellationTokenSource cancellationTokenSource;
        private UdpClient qsoUdpClient;
        private UdpClient heartbeatUdpClient;

        public UdpClientListener(ListenerConfig listenerConfig, UdpClient? forwardUdpClient, BlockingCollection<QsoMessage> qsoMessageQueue,
                                 ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource)
        {
            this.listenerConfig = listenerConfig;
            this.forwardUdpClient = forwardUdpClient;
            this.qsoMessageQueue = qsoMessageQueue;
            this.progressUpdater = progressUpdater;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public async Task Start()
        {
            Task.Run(() => StartHeartbeat());

            int qsoPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new(qsoPort);
            qsoUdpClient = udpClient;
            string logMessage = $"UDP Port {qsoPort} ({listenerConfig.Name}) QSO Listener started";
            log.Information(logMessage);
            progressUpdater.UpdateLog(logMessage);
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    log.Debug("Waiting for UDP message on port {Port} ({ListenerName}:{Format})", 
                        qsoPort, listenerConfig.Name, listenerConfig.MessageFormat);

                    var receivedResults = await qsoUdpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    
                    log.Debug("UDP message received on port {Port} ({ListenerName}:{Format}), length: {Length} bytes", 
                        qsoPort, listenerConfig.Name, listenerConfig.MessageFormat, receivedBytes.Length);
                    if (forwardUdpClient != null)
                    {
                        log.Debug("Forwarding UDP message from port {Port} ({ListenerName}:{Format}) to port {ForwardPort}, length: {Length} bytes", 
                            qsoPort, listenerConfig.Name, listenerConfig.MessageFormat, listenerConfig.ForwardPort.Value, receivedBytes.Length);
                    
                        await forwardUdpClient.SendAsync(receivedBytes, receivedBytes.Length);
                        string forwardLogMessage = $"QSO info from {listenerConfig.Name} forwarded to port {listenerConfig.ForwardPort.Value}";
                        log.Debug(forwardLogMessage);
                        progressUpdater.UpdateLog(forwardLogMessage);
                    }
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);
                    log.Debug("UDP message received on port {Port} ({ListenerName}:{Format}): {ReceivedData}", 
                        qsoPort, listenerConfig.Name, listenerConfig.MessageFormat, receivedData);

                    QsoMessage qsoMessage = new() { Source = listenerConfig.Name, OriginalFormat = listenerConfig.MessageFormat, OriginalQsoData = receivedData };
                    if (!IsExpectedMessageFormat(qsoMessage)) {
                        continue;
                    }
                    log.Debug("Add to queue UDP message on port {Port} ({ListenerName}:{Format})", 
                        qsoPort, listenerConfig.Name, listenerConfig.MessageFormat);
                    qsoMessageQueue.Add(qsoMessage);
                    progressUpdater.UpdateProgress(true, false, false, false, $"QSO received on port {qsoPort} ({listenerConfig.Name}:{listenerConfig.MessageFormat})");
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }

                catch (Exception ex)
                {
                    qsoUdpClient.Close();
                    qsoUdpClient.Dispose();
                    if (forwardUdpClient != null) {
                        forwardUdpClient.Close();
                        forwardUdpClient.Dispose();
                    }
                    cancellationTokenSource.Dispose();
                    
                    string message;
                    if (ex is OperationCanceledException)
                    {
                        message = $"UDP listener on port {qsoPort} ({listenerConfig.Name}:{listenerConfig.MessageFormat}) was stopped";
                        log.Information(message);
                    }
                    else
                    {
                        message = $"!!!UDP listener on port {qsoPort} ({listenerConfig.Name}:{listenerConfig.MessageFormat}) unexpectedly stopped: {ex.Message}";
                        log.Error(ex, message);
                    }
                    progressUpdater.UpdateLog(message);
                    break;
                }
            }
        }

        private bool IsExpectedMessageFormat(QsoMessage qsoMessage)
        {
            string[] requiredTexts = listenerConfig.MessageFormat switch
            {
                "ADIF" => ["<EOR>", "<QSO_DATE:"],
                "N1MM" => ["<contactinfo>", "</contactinfo>"],
                _ => []
            };
            foreach (string text in requiredTexts)
            {
                if (!qsoMessage.OriginalQsoData.Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    string logMessage = $"Warning: Received QSO message does not appear to be in expected format ({listenerConfig.MessageFormat}). Ignoring";
                    log.Warning(logMessage);
                    progressUpdater.UpdateLog(logMessage);
                    return false;
                }
            }
            return true;
        }

        private async Task StartHeartbeat()
        {
            int? heartbeatPort = listenerConfig.AcknowledgePort;
            if (heartbeatPort == null) return;

            using UdpClient udpClient = new(heartbeatPort.Value);
            heartbeatUdpClient = udpClient;
            progressUpdater.UpdateLog($"UDP Port {heartbeatPort} ({listenerConfig.Name}) Acknowledge listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    log.Debug("Waiting for heartbeat message on port {Port} ({ListenerName})", heartbeatPort, listenerConfig.Name);
                    var receivedResults = await heartbeatUdpClient.ReceiveAsync(cancellationToken);
                    string logMessage = $"Received heartbeat message on port {heartbeatPort} ({listenerConfig.Name})";
                    log.Debug(logMessage);
                    progressUpdater.UpdateLog(logMessage, true);
                    //byte[] receivedBytes = receivedResults.Buffer;
                    //string receivedData = Encoding.UTF8.GetString(receivedBytes);
                    //progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    heartbeatUdpClient.Close();
                    heartbeatUdpClient.Dispose();
                    cancellationTokenSource.Dispose();

                    string message;
                    if (ex is OperationCanceledException)
                    {
                        message = $"UDP Port {heartbeatPort} ({listenerConfig.Name}) Heartbeat listener was stopped";
                        log.Information(message);
                    }
                    else
                    {
                        message = $"!!!UDP Port {heartbeatPort} ({listenerConfig.Name}) Heartbeat listener unexpectedly stopped";
                        log.Error(ex, message);
                    }
                    progressUpdater.UpdateLog(message);
                    break;
                }
            }
        }

        public void Stop()
        {
            log.Debug("Stopping UDP listener on port {Port} ({ListenerName}:{Format})", 
                listenerConfig.QsoPort, listenerConfig.Name, listenerConfig.MessageFormat);
            cancellationTokenSource.Cancel();
        }
    }
}