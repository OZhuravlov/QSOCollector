using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace QSOCollector
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

            int localPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new (localPort);
            qsoUdpClient = udpClient;
            progressUpdater.UpdateLog($"UDP Port {localPort} ({listenerConfig.Name}) QSO Listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            if (listenerConfig.ForwardPort != null)
            {
                forwardUdpClient = new UdpClient();
                forwardUdpClient.Connect("localhost", listenerConfig.ForwardPort.Value);
                progressUpdater.UpdateLog($"UDP Port {localPort} ({listenerConfig.Name}) Forwarding to port {listenerConfig.ForwardPort.Value}");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receivedResults = await qsoUdpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    if (listenerConfig.ForwardPort != null)
                    {
                        if (forwardUdpClient != null)
                        {
                            await forwardUdpClient.SendAsync(receivedBytes, receivedBytes.Length);
                        }
                    }
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { Source = listenerConfig.Name, OriginalFormat = listenerConfig.MessageFormat, OriginalQsoData = receivedData };
                    qsoMessageQueue.Add(qsoMessage);
                    progressUpdater.UpdateProgress(true, false, false, false, $"QSO received on port {localPort} ({listenerConfig.Name})");
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    qsoUdpClient.Close();
                    qsoUdpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    string message = (ex is OperationCanceledException)
                        ? $"UDP Port {localPort} ({listenerConfig.Name}) QSO Listener was stopped"
                        : $"!!!UDP Port {localPort} ({listenerConfig.Name}) QSO Listener unexpectedly stopped";
                    progressUpdater.UpdateLog(message);
                    break;
                }
            }
        }

        private async Task StartAcknowledge()
        {
            int? localPort = listenerConfig.AcknowledgePort;
            if (localPort == null) return;

            using UdpClient udpClient = new(localPort.Value);
            acknowledgeUdpClient = udpClient;
            progressUpdater.UpdateLog($"UDP Port {localPort} ({listenerConfig.Name}) Acknowledge listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receivedResults = await acknowledgeUdpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    progressUpdater.UpdateProgress(true, false, false, false, $"App info received on port {localPort} ({listenerConfig.Name})");
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    acknowledgeUdpClient.Close();
                    acknowledgeUdpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    string message = (ex is OperationCanceledException)
                        ? $"UDP Port {localPort} ({listenerConfig.Name}) Acknowledge listener was stopped"
                        : $"!!!UDP Port {localPort} ({listenerConfig.Name}) Acknowledge listener unexpectedly stopped";
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