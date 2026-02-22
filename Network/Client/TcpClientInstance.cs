using QSOCollector.Helpers;
using QSOCollector.Models;
using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector.Network.Client
{
    internal class TcpClientInstance
    {
        private readonly TcpClient? client;
        private NetworkStream? stream;
        private StreamReader? r;
        private StreamWriter? w;
        private readonly ClientProgressUpdater progressUpdater;

        public TcpClientInstance(string ipAddress, int port, ClientProgressUpdater progressUpdater)
        {
            client = new TcpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Connect(ipAddress, port);
            Socket socket = client.Client;
            /*
            uint on = 1;
            uint keepAliveTime = 30000; // 30 seconds
            uint keepAliveInterval = 1000; // 1 second
            byte[] inValue = new byte[12];
            BitConverter.GetBytes(on).CopyTo(inValue, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(inValue, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(inValue, 8);
            */
            // 30 seconds idle time and 1 second interval
            socket.IOControl(IOControlCode.KeepAliveValues, [1, 0, 0, 0, 0xE8, 0x03, 0x00, 0x00, 0xE8, 0x03, 0x00, 0x00], null);
            this.progressUpdater = progressUpdater;
        }

        public bool IsConnected()
        {
            return client != null && client.Connected;
        }

        public async Task SendMessage(string qsoMessage, int responseDelay, string source, bool isTest)
        {
            qsoMessage = qsoMessage.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(qsoMessage)) return;

            if (!IsConnected())
            {
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
            if (!isTest)
            {
                progressUpdater.UpdateProgress(false, true, false, false, $"QSO from {source} sent to server");
            }
            else
            {
                progressUpdater.UpdateLog("Server status requested", true);
            }

            string? responseMessage = await r.ReadLineAsync(new CancellationTokenSource(responseDelay).Token);
            ServerResponse serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseMessage);
            if (serverResponse == null)
            {
                progressUpdater.UpdateLog($"Server response timeout");
                return;
            }

            if (serverResponse.Status != ServerResponseStatus.Ok)
            {
                progressUpdater.UpdateLog($"Server returned an error: {responseMessage}");
                return;
            }

            string responseDetailedMessage = isTest ? $"Server status: {serverResponse.Status}" : $"QSO from {source} processed by server";
            progressUpdater.UpdateLog(responseDetailedMessage, true);
            progressUpdater.UpdateServerStatus("Active", null);
        }

        public void Terminate()
        {
            if (client != null)
            {
                client.Close();
                client.Dispose();
            }
        }
    }
}
