using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QSOCollector
{
    public class UdpClientListener
    {
        private readonly int localPort;
        private readonly string serverIp;
        private readonly int serverPort;
        private readonly TextBox logTextBox;

        public UdpClientListener(int localPort, string serverIp, int serverPort, TextBox logTextBox)
        {
            this.localPort = localPort;
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.logTextBox = logTextBox;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (UdpClient udpClient = new UdpClient(localPort))
            {
                logTextBox.Invoke((MethodInvoker)delegate
                {
                    logTextBox.AppendText($"UDP Port {localPort} Listener started\r\n");
                });
                TcpClientInstance tcpClient = new TcpClientInstance(serverIp, serverPort);
                bool continueReceiving = true;
                while (continueReceiving)
                {
                    try
                    {
                        UdpReceiveResult receivedResults = await udpClient.ReceiveAsync(cancellationToken);
                        byte[] receivedBytes = receivedResults.Buffer;
                        string receivedData = Encoding.UTF8.GetString(receivedBytes);

                        logTextBox.Invoke((MethodInvoker)delegate
                        {
                            logTextBox.AppendText(receivedData);
                            logTextBox.AppendText("\r\n");
                        });

                        await tcpClient.SendMessage(receivedData, logTextBox);
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
}