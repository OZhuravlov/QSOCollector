using System.Net.Sockets;
using System.Text.Json;


namespace QSOCollector
{
    internal class TcpClientInstance
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader r;
        private StreamWriter w;

        public TcpClientInstance(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
        }

        public bool isConnected() {
            return client.Connected;
        }

        public async Task SendMessage(string qsoMessage, TextBox clientLogTextBox)
        {
            qsoMessage = qsoMessage.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(qsoMessage)) return;

            if (!client.Connected) {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            if (stream == null)
            {
                stream = client.GetStream();
                r = new StreamReader(stream);
                w = new StreamWriter(stream);
                w.AutoFlush = true;
            }

            w.WriteLine(qsoMessage);
            string? responseMessage = await r.ReadLineAsync(new CancellationTokenSource(100000).Token);
            ServerResponse serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseMessage);

            clientLogTextBox.Invoke((MethodInvoker)delegate
            {
                clientLogTextBox.AppendText(serverResponse.ToString());
                clientLogTextBox.AppendText("\r\n");
            });
        }

        public void Terminate() {
            client.Close();
            client.Dispose();
        }
    }
}
