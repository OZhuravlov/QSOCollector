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
            log.Information("Connecting to server at {ServerIp}:{ServerPort}", ipAddress, port);
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
            log.Information("TCP client connected to server at {ServerIp}:{ServerPort}", ipAddress, port);
        }

        public bool IsConnected()
        {
            bool isConnected = client != null && client.Connected;
            log.Debug("Is TCP client connection status: {isConnected}", isConnected);
            return isConnected;
        }

        public async Task SendMessage(QsoMessage qsoMessage, int responseDelay)
        {
            string qsoMessageJson = JsonSerializer.Serialize(qsoMessage);
            qsoMessageJson = qsoMessageJson.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();
            if (string.IsNullOrEmpty(qsoMessageJson)) return;

            if (!IsConnected())
            {
                string logMessage = "TCP client is not connected to server, cannot send message. Closing client";
                log.Warning(logMessage);
                throw new SocketException((int)SocketError.ConnectionAborted, logMessage);
            }

            if (stream == null)
            {
                log.Information("Initializing network stream and reader/writer for TCP client");
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream)
                {
                    AutoFlush = true
                };
            }

            if (!qsoMessage.IsHeartBeat)
            {
                log.Debug("Sending message to server: {Source}:{Format}", qsoMessage.Source, qsoMessage.OriginalFormat);
                writer.WriteLine(qsoMessageJson);
                string logMessage = $"QSO from {qsoMessage.Source} sent to server";
                log.Information(logMessage);
                log.Debug("Sent message: {qsoMessage}", qsoMessage.OriginalQsoData);
                progressUpdater.UpdateProgress(false, true, false, false, logMessage);
            }
            else
            {
                log.Debug("Sending Heartbeat message to server");
                writer.WriteLine(qsoMessageJson);
                string logMessage = "Server status requested";
                log.Debug(logMessage);
                progressUpdater.UpdateLog(logMessage, true);
            }

            log.Debug("Waiting for server response with a timeout of {ResponseDelay} ms", responseDelay);
            string? responseMessage = await reader.ReadLineAsync(new CancellationTokenSource(responseDelay).Token);
            log.Debug("Received server response: {responseMessage}", responseMessage);
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

            string responseDetailedMessage = qsoMessage.IsHeartBeat ? $"Server status: {serverResponse.Status}" : $"QSO from {qsoMessage.Source} processed by server";
            log.Debug(responseDetailedMessage);
            progressUpdater.UpdateLog(responseDetailedMessage, true);
            progressUpdater.UpdateServerStatus("Active", null);
        }

        public void Terminate()
        {
            log.Information("Terminating TCP client requested");
            if (client != null)
            {
                log.Information("Terminating TCP client connection");
                client.Close();
                client.Dispose();
            }
            else
            {
                log.Warning("TCP client is already terminated");
            }
        }
    }
}
