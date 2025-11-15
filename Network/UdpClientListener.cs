using QSOCollector.Helpers;
using QSOCollector.Models;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace QSOCollector.Network
{
    public class UdpClientListener
    {

        private ListenerConfig listenerConfig;
        private BlockingCollection<QsoMessage> qsoMessageQueue;
        private ClientProgressUpdater progressUpdater;
        private CancellationTokenSource cancellationTokenSource;
        private UdpClient qsoUdpClient;
        private UdpClient forwardUdpClient;
        private UdpClient acknowledgeUdpClient;

        public UdpClientListener(ListenerConfig listenerConfig, BlockingCollection<QsoMessage> qsoMessageQueue,
                                 ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource)
        {
            this.listenerConfig = listenerConfig;
            this.qsoMessageQueue = qsoMessageQueue;
            this.progressUpdater = progressUpdater;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public async Task Start()
        {
            Task.Run(() => StartAcknowledge());

            int qsoPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new(qsoPort);
            qsoUdpClient = udpClient;
            progressUpdater.UpdateLog($"UDP Port {qsoPort} ({listenerConfig.Name}) QSO Listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            if (listenerConfig.ForwardPort != null)
            {
                forwardUdpClient = new UdpClient();
                forwardUdpClient.Connect("localhost", listenerConfig.ForwardPort.Value);
                progressUpdater.UpdateLog($"UDP Port {qsoPort} ({listenerConfig.Name}) Forwarding to port {listenerConfig.ForwardPort.Value}");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receivedResults = await qsoUdpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    if (forwardUdpClient != null)
                    {
                        await forwardUdpClient.SendAsync(receivedBytes, receivedBytes.Length);
                    }
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { Source = listenerConfig.Name, OriginalFormat = listenerConfig.MessageFormat, OriginalQsoData = receivedData };
                    if (!IsExpectedMessageFormat(qsoMessage)) {
                        continue;
                    }
                    
                    qsoMessageQueue.Add(qsoMessage);
                    progressUpdater.UpdateProgress(true, false, false, false, $"QSO received on port {qsoPort} ({listenerConfig.Name})");
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    qsoUdpClient.Close();
                    qsoUdpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    string message = ex is OperationCanceledException
                        ? $"UDP Port {qsoPort} ({listenerConfig.Name}) QSO Listener was stopped"
                        : $"!!!UDP Port {qsoPort} ({listenerConfig.Name}) QSO Listener unexpectedly stopped";
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
                    progressUpdater.UpdateLog($"Warning: Received QSO message does not appear to be in expected format ({listenerConfig.MessageFormat}). Ignoring", true);
                    return false;
                }
            }
            return true;
        }

        private async Task StartAcknowledge()
        {
            int? acknowledgePort = listenerConfig.AcknowledgePort;
            if (acknowledgePort == null) return;

            using UdpClient udpClient = new(acknowledgePort.Value);
            acknowledgeUdpClient = udpClient;
            progressUpdater.UpdateLog($"UDP Port {acknowledgePort} ({listenerConfig.Name}) Acknowledge listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receivedResults = await acknowledgeUdpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    progressUpdater.UpdateLog($"App info received on port {acknowledgePort} ({listenerConfig.Name})", true);
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    acknowledgeUdpClient.Close();
                    acknowledgeUdpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    string message = ex is OperationCanceledException
                        ? $"UDP Port {acknowledgePort} ({listenerConfig.Name}) Acknowledge listener was stopped"
                        : $"!!!UDP Port {acknowledgePort} ({listenerConfig.Name}) Acknowledge listener unexpectedly stopped";
                    progressUpdater.UpdateLog(message);
                    break;
                }
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}