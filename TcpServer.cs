using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;

namespace QSOCollector
{
    internal class TcpServer
    {
        private IPEndPoint ipEndPoint;
        private TcpListener listener;
        private bool Running;
        private List<Client> clients = new();
        private CancellationTokenSource cts;

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
                try {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
                    Client client = new(tcpClient, cts.Token);
                    clients.Add(client);
                    Task clientTask = client.Run(connectionString, serverLogTextBox); //don't await
                    clientTask.ContinueWith(t => clients.Remove(client));
                }
                catch (OperationCanceledException) { 
                    listener.Stop();
                    listener.Dispose();
                    break;
                }
            }
        }
    }

    internal class Client
    {
        private NetworkStream stream;
        private CancellationToken token;

        public Client(TcpClient client, CancellationToken token)
        {
            this.token = token;
            stream = client.GetStream();
        }

        public async Task Run(string connectionString, TextBox serverLogTextBox)
        {
            try
            {
                using (DbConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    StreamReader r = new(stream);
                    StreamWriter w = new(stream);
                    w.AutoFlush = true;
                    while (true)
                    {
                        string? qsoMessage = await r.ReadLineAsync(token);
                        if (qsoMessage == null) break;
                        using (DbCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO QSOs (QSOData) VALUES (@qsoData)";
                            DbParameter param = command.CreateParameter();
                            param.ParameterName = "@qsoData";
                            param.Value = qsoMessage;
                            command.Parameters.Add(param);
                            await command.ExecuteNonQueryAsync(token);
                        }
                        serverLogTextBox.Invoke((MethodInvoker)delegate
                        {
                            serverLogTextBox.AppendText(qsoMessage);
                            serverLogTextBox.AppendText("\r\n");
                            serverLogTextBox.AppendText("Saved to Database\r\n");
                            serverLogTextBox.AppendText("\r\n");
                        });
                        await w.WriteLineAsync("Saved to Database");
                    }
                }
            }
            catch (SqliteException)
            {
                serverLogTextBox.Invoke((MethodInvoker)delegate
                {
                    serverLogTextBox.AppendText("Server database is not available. Please make sure it hasn't been deleted by mistake\r\n");
                });
            }
        }
    }
}
