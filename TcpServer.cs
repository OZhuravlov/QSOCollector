using Microsoft.Data.Sqlite;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector
{
    internal class TcpServer
    {
        private readonly IPEndPoint ipEndPoint;
        private TcpListener? listener;
        private bool Running;
        private readonly List<Client> clients = [];
        private readonly CancellationTokenSource cts;

        public TcpServer(int port)
        {
            IPAddress ip = IPAddress.Loopback;
            ipEndPoint = new(ip, port);
            cts = new CancellationTokenSource();
        }

        public void Stop()
        {
            Running = false;
            cts.Cancel();
        }

        public async Task Start(string connectionString, TextBox serverLogTextBox)
        {
            await Run(connectionString, serverLogTextBox);
        }

        private async Task Run(string connectionString, TextBox serverLogTextBox)
        {
            listener = new(ipEndPoint);
            listener.Start();
            Running = true;
            while (Running)
            {
                try
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
                    Client client = new(tcpClient, cts.Token, serverLogTextBox);
                    clients.Add(client);
                    Task clientTask = client.Run(connectionString); //don't await
                    clientTask.ContinueWith(t => clients.Remove(client));
                }
                catch (OperationCanceledException)
                {
                    listener.Stop();
                    listener.Dispose();
                    break;
                }
            }
        }
    }

    internal class Client(TcpClient client, CancellationToken token, TextBox serverLogTextBox)
    {
        private NetworkStream stream = client.GetStream();
        private readonly string clientIPAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

        public async Task Run(string dbConnectionString)
        {
            DbRepository dbRepository = new(dbConnectionString);
            StreamReader r = new(stream);
            StreamWriter w = new(stream)
            {
                AutoFlush = true
            };

            while (true)
            {
                string? qsoMessageJson = await r.ReadLineAsync(token);
                if (qsoMessageJson == null) continue;
                QsoMessage? qsoMessage = JsonSerializer.Deserialize<QsoMessage>(qsoMessageJson);
                if (qsoMessage == null) continue;

                ServerResponse response;
                try
                {
                    List<Dictionary<string, string>> qsoRecords = parseAdif(qsoMessage, serverLogTextBox);
                    if (qsoRecords.Count == 0)
                    {
                        throw new ArgumentException("No valid QSO records found in ADIF data");
                    }

                    dbRepository.SaveQsoRecords(qsoRecords, isImported: false, isTemporary: false);
                    response = new ServerResponse(ServerResponseStatus.Ok);
                }
                catch (SqliteException ex)
                {
                    response = new ServerResponse(ServerResponseStatus.SqliteError, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    response = new ServerResponse(ServerResponseStatus.ArgumentError, ex.Message);
                }
                catch (Exception ex)
                {
                    response = new ServerResponse(ServerResponseStatus.UnknownError, ex.Message);
                }

                LogToTextBox(response.ToString());
                await w.WriteLineAsync(JsonSerializer.Serialize<ServerResponse>(response));
            }
        }

        private List<Dictionary<string, string>> parseAdif(QsoMessage qsoMessage, TextBox serverLogTextBox)
        {
            List<Dictionary<string, string>> qsoRecords = [];
            try
            {
                // Parse the ADIF message
                qsoRecords = AdifParser.Parse(qsoMessage, clientIPAddress);

                // Log parsed records
                    foreach (var record in qsoRecords)
                    {
                    StringBuilder logMessage = new StringBuilder();
                        foreach (var kv in record)
                        {
                           logMessage.AppendLine(kv.Key + ": " + kv.Value);
                        }
                    logMessage.AppendLine("----");
                    LogToTextBox(logMessage.ToString());
                }
                LogToTextBox("----");
            }
            catch (Exception ex)
            {
                // Handle parsing errors
                LogToTextBox($"ADIF parsing error: {ex.Message}");
                throw new ArgumentException("ADIF parsing error", ex);
            }
            return qsoRecords;
        }

        private void LogToTextBox(String message)
        {
            serverLogTextBox.Invoke((MethodInvoker)delegate
            {
                serverLogTextBox.AppendText($"{message}\r\n");
            });
        }
    }
}
