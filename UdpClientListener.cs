using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector
{
    public class UdpClientListener(ListenerConfig listenerConfig, string serverIp, int serverPort, TextBox logTextBox)
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int localPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new (localPort);
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText($"UDP Port {localPort} Listener started\r\n");
            });
            TcpClientInstance tcpClient = new (serverIp, serverPort);
            bool continueReceiving = true;
            while (continueReceiving)
            {
                try
                {
                    var receivedResults = await udpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    string receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { OriginalFormat = listenerConfig.MessageFormat, AdifData = receivedData };
                    logTextBox.Invoke((MethodInvoker)delegate
                    {
                        logTextBox.AppendText(receivedData);
                        logTextBox.AppendText("\r\n");
                    });

                    string qsoMessageJson = JsonSerializer.Serialize<QsoMessage>(qsoMessage);
                    await tcpClient.SendMessage(qsoMessageJson, logTextBox);
                }
                catch (OperationCanceledException)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    logTextBox.Invoke((MethodInvoker)delegate
                    {
                        logTextBox.AppendText($"UDP Port {localPort} Listener stopped\r\n");
                        continueReceiving = false;
                    });
                }
            }
        }
    }
}