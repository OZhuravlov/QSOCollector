using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector
{
    internal class TcpClientInstance
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private StreamReader? r;
        private StreamWriter? w;

        public TcpClientInstance(string ipAddress, int port)
        {
            client = new TcpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Connect(ipAddress, port);
        }

        public bool IsConnected() {
            return client != null && client.Connected;
        }

        public async Task SendMessage(string qsoMessage, TextBox clientLogTextBox, int responseDelay)
        {
            qsoMessage = qsoMessage.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(qsoMessage)) return;

            if (!IsConnected()) {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            if (stream == null)
            {
                stream = client.GetStream();
                r = new StreamReader(stream);
                w = new StreamWriter(stream)
                {
                    AutoFlush = true
                };
            }

            w.WriteLine(qsoMessage);
            string? responseMessage = await r.ReadLineAsync(new CancellationTokenSource(responseDelay).Token);
            ServerResponse serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseMessage);
            LogToTextBox(clientLogTextBox, serverResponse.ToString());
        }

        public void Terminate() {
            if (client != null)
            {
                client.Close();
                client.Dispose();
            }
        }

        private void LogToTextBox(TextBox logTextBox, String message)
        {
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText($"{message}\r\n");
            });
        }
    }
}
