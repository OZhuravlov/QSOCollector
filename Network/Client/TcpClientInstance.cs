using QSOCollector.Helpers;
using QSOCollector.Models;
using Serilog;
using System.Net.Sockets;
using System.Text.Json;

namespace QSOCollector.Network.Client
{
    internal class TcpClientInstance
    {
        private readonly ILogger log = Log.ForContext<TcpClientInstance>();

        private readonly TcpClient? client;
        private NetworkStream? stream;
        private StreamReader? reader;
        private StreamWriter? writer;
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

        public async Task SendMessage(QsoMessage qsoMessage, int responseDelay)
        {
            string qsoMessageJson = JsonSerializer.Serialize(qsoMessage);
            qsoMessageJson = qsoMessageJson.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(qsoMessageJson)) return;

            if (!IsConnected())
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            if (stream == null)
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream)
                {
                    AutoFlush = true
                };
            }

            writer.WriteLine(qsoMessageJson);
            if (!qsoMessage.IsTest)
            {
                string logMessage = $"QSO from {qsoMessage.Source} sent to server";
                log.Information(logMessage);
                log.Debug("Sent message: {qsoMessage}", qsoMessage.OriginalQsoData);
                progressUpdater.UpdateProgress(false, true, false, false, logMessage);
            }
            else
            {
                string logMessage = "Server status requested";
                log.Debug(logMessage);
                progressUpdater.UpdateLog(logMessage, true);
            }

            string? responseMessage = await reader.ReadLineAsync(new CancellationTokenSource(responseDelay).Token);
            ServerResponse serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseMessage);
            if (serverResponse == null)
            {
                string logMessage = "Server response timeout";
                log.Warning(logMessage);
                progressUpdater.UpdateLog(logMessage);
                return;
            }

            if (serverResponse.Status != ServerResponseStatus.Ok)
            {
                string logMessage = $"Server returned an error: {responseMessage}";
                log.Error(logMessage);
                progressUpdater.UpdateLog(logMessage);
                return;
            }

            string responseDetailedMessage = qsoMessage.IsTest ? $"Server status: {serverResponse.Status}" : $"QSO from {qsoMessage.Source} processed by server";
            log.Debug(responseDetailedMessage);
            progressUpdater.UpdateLog(responseDetailedMessage, true);
            progressUpdater.UpdateServerStatus("Active", null);
        }

        public void Terminate()
        {
            if (client != null)
            {
                log.Information("Terminating TCP client connection");
                client.Close();
                client.Dispose();
            }
        }
    }
}
