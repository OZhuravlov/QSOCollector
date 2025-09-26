using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace QSOCollector
{
    public class UdpClientListener(
        ListenerConfig listenerConfig, BlockingCollection<QsoMessage> qsoMessageQueue, 
        TextBox logTextBox, CancellationTokenSource cancellationTokenSource)
    {
        public async Task Start()
        {
            int localPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new(localPort);
            LogToTextBox($"UDP Port {localPort} Listener started");
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                string receivedData = string.Empty;
                try
                {
                    var receivedResults = await udpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { OriginalFormat = listenerConfig.MessageFormat, OriginalQsoData = receivedData };
                    LogToTextBox(receivedData);
                    qsoMessageQueue.Add(qsoMessage);
                }
                catch (OperationCanceledException)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    LogToTextBox($"UDP Port {localPort} Listener was stopped");
                    break;
                }
                catch (Exception)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    cancellationTokenSource.Dispose();
                    LogToTextBox($"!!!UDP Port {localPort} Listener unexpectedly stopped");
                    break;
                }
            }
        }

        private void LogToTextBox(String message)
        {
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText($"{message}\r\n");
            });
        }
    }
}