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

        public TcpClientInstance(ClientProgressUpdater progressUpdater)
        {
            client = new TcpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.progressUpdater = progressUpdater;
        }

        // Use async factory to create and connect an instance
        public static async Task<TcpClientInstance> CreateAsync(string ipAddress, int port, ClientProgressUpdater progressUpdater, int connectTimeoutMs = 5000)
        {
            var instance = new TcpClientInstance(progressUpdater);
            try
            {
                await instance.ConnectAsyncWithTimeout(ipAddress, port, connectTimeoutMs);
                return instance;
            }
            catch
            {
                try
                {
                    instance.client?.Close();
                    instance.client?.Dispose();
                }
                catch { }
                throw;
            }
        }

        public bool IsConnected()
        {
            //try
            //{
                if (client == null) return false;
                Socket? socket = client.Client;
                if (socket == null) return false;

                // If the socket reports not connected, we're done
                if (!socket.Connected) return false;
                return ProbeSocket(socket);

                // The Connected property may be true even after the remote end has closed the connection.
                // Use Poll to check for readability: if Poll returns true for Read and no data is available
                // (Available == 0) then the socket has been closed/reset by the remote.
                /*bool hasDataOrStillConnected = true;
                try
                {
                    // If there is an error condition reported, consider disconnected
                    if (socket.Poll(0, SelectMode.SelectError))
                    {
                        return false;
                    }

                    // check readability: if socket is readable but no data available -> closed by remote
                    bool canRead = socket.Poll(0, SelectMode.SelectRead);
                    bool noData = socket.Available == 0;
                    if (canRead && noData)
                    {
                        // socket closed
                        hasDataOrStillConnected = false;
                    }
                    else if (canRead && socket.Available > 0)
                    {
                        // Data is available; do a non-destructive peek to see if remote closed (Receive returns 0)
                        try
                        {
                            byte[] peekBuffer = new byte[1];
                            int peeked = socket.Receive(peekBuffer, 0, 1, SocketFlags.Peek);
                            if (peeked == 0)
                            {
                                hasDataOrStillConnected = false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    // additionally check writability: if socket is not writable then consider disconnected
                    bool canWrite = socket.Poll(0, SelectMode.SelectWrite);
                    if (!canWrite)
                    {
                        hasDataOrStillConnected = false;
                    }
                }
                catch
                {
                    return false;
                }

                return hasDataOrStillConnected;
            }
            catch
            {
                return false;
            }*/
            }

        private static bool ProbeSocket(Socket socket)
        {
            try
            {
                // Attempt a zero-length send. On a broken connection this may throw a SocketException.
                // This is a heuristic: behavior can vary by platform, but it often detects a dead peer
                // when other checks (Poll/Available) are inconclusive.
                socket.Send(Array.Empty<byte>());
                return true;
        }
            catch (SocketException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendMessage(string qsoMessage, int writeTimeoutMs, int responseDelayMs, string source, bool isTest)
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
                // do not create StreamWriter - use NetworkStream.WriteAsync for cancellable writes
                w = null;
            }

            // Write with timeout using NetworkStream.WriteAsync and a cancellation token
            try
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(qsoMessage + "\n");
                using var writeCts = new CancellationTokenSource(writeTimeoutMs);
                await stream.WriteAsync(buffer.AsMemory(0, buffer.Length), writeCts.Token);
            }
            catch (OperationCanceledException)
            {
                progressUpdater.UpdateLog("Sending message to server timed out");
                return;
            }
            catch (Exception ex)
                {
                progressUpdater.UpdateLog($"Error sending message to server: {ex.Message}");
                throw;
            }

            if (!isTest)
            {
                w.WriteLine(qsoMessage);
                progressUpdater.UpdateProgress(false, true, false, false, $"QSO from {source} sent to server");
            }
            else
            {
                progressUpdater.UpdateLog("Server status requested", true);
            }

            // Read response with cancellation (so underlying stream read can be cancelled)
            string? responseMessage = null;
            try
            {
                using var cts = new CancellationTokenSource(responseDelayMs);
                responseMessage = await r.ReadLineAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                progressUpdater.UpdateServerStatus("Unavailable", "Server response timeout");
                return;
            }

            ServerResponse? serverResponse = null;
            try
            {
                serverResponse = JsonSerializer.Deserialize<ServerResponse>(responseMessage);
            }
            catch (JsonException)
            {
                progressUpdater.UpdateLog($"Server returned invalid response: {responseMessage}");
                return;
            }

            if (serverResponse == null)
            {
                progressUpdater.UpdateLog("Server returned empty response");
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

        private async Task ConnectAsyncWithTimeout(string ipAddress, int port, int timeoutMs)
        {
            if (client == null) throw new InvalidOperationException("TcpClient is not initialized");
            var connectTask = client.ConnectAsync(ipAddress, port);
            var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs));
            if (completed != connectTask)
            {
                // timed out
                throw new TimeoutException("TcpClient.ConnectAsync timed out");
            }
            // propagate exceptions if any
            await connectTask;
        }

        public void Terminate()
        {
            if (client != null)
            {
                try
                {
                    r?.Dispose();
                }
                catch { }
                try
                {
                    w?.Dispose();
                }
                catch { }
                try
                {
                    stream?.Dispose();
                }
                catch { }
                try
                {
                client.Close();
                client.Dispose();
            }
                catch { }
            }
        }
    }
}
