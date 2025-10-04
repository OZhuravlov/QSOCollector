using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace QSOCollector
{
    public class UdpClientListener(
        ListenerConfig listenerConfig, BlockingCollection<QsoMessage> qsoMessageQueue, 
        ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource)
    {
        public async Task Start()
        {
            int localPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new(localPort);
            progressUpdater.UpdateLog($"UDP Port {localPort} ({listenerConfig.Description}) Listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receivedResults = await udpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { Source = listenerConfig.Description, OriginalFormat = listenerConfig.MessageFormat, OriginalQsoData = receivedData };
                    qsoMessageQueue.Add(qsoMessage);
                    progressUpdater.UpdateProgress(true, false, false, false, $"QSO received on port {localPort} ({listenerConfig.Description})");
                    progressUpdater.UpdateLog($"Data: {receivedData}", true);
                }
                catch (Exception ex)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    string message = (ex is OperationCanceledException)
                        ? $"UDP Port {localPort} ({listenerConfig.Description}) Listener was stopped"
                        : $"!!!UDP Port {localPort} ({listenerConfig.Description}) Listener unexpectedly stopped";
                    progressUpdater.UpdateLog(message);
                    break;
                }
            }
        }
    }
}