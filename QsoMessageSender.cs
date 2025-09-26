using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector
{
    public class QsoMessageSender
    {
        private const int responseTimeoutMs = 5000;

        private readonly string serverIp;
        private readonly int serverPort;
        private readonly BlockingCollection<QsoMessage> qsoMessageQueue;
        private readonly DbRepository dbRepository;
        private readonly TextBox logTextBox;
        private readonly CancellationTokenSource cancellationTokenSource;
        private TcpClientInstance? tcpClient;

        public QsoMessageSender(string serverIp, int serverPort, BlockingCollection<QsoMessage> qsoMessageQueue, 
            DbRepository dbRepository, TextBox logTextBox, CancellationTokenSource cancellationTokenSource)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.qsoMessageQueue = qsoMessageQueue;
            this.dbRepository = dbRepository;
            this.logTextBox = logTextBox;
            this.cancellationTokenSource = cancellationTokenSource;
            try
            {
                tcpClient = new TcpClientInstance(serverIp, serverPort);
            }
            catch (SocketException ex) {
                tcpClient = null;
                LogToTextBox($"Unexpected error while creating QSO sender: {ex.Message}");
            }
            
        }

        public bool IsConnected()
        {
            return tcpClient != null && tcpClient.IsConnected();
        }

        public async Task Start()
        {
            while (true)
            {
                QsoMessage qsoMessage;
                try
                {
                    qsoMessage = qsoMessageQueue.Take();
                }
                catch (InvalidOperationException)
                {
                    LogToTextBox("Qso Message handler has been stopped");
                    return;
                }

                try
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        SaveTemporarelyToDb(qsoMessage, "Client to be stopped requested");
                    }
                    else
                    {
                        string qsoMessageJson = JsonSerializer.Serialize<QsoMessage>(qsoMessage);
                        try
                        {
                            if (!tcpClient.IsConnected())
                            {
                                tcpClient = new(serverIp, serverPort);
                            }
                            await tcpClient.SendMessage(qsoMessageJson, logTextBox, responseTimeoutMs);
                        }
                        catch (OperationCanceledException ex)
                        {
                            SaveTemporarelyToDb(qsoMessage, $"Error sending to server: {ex.Message}");
                        }
                        catch (SocketException ex)
                        {
                            SaveTemporarelyToDb(qsoMessage, $"Socket Connection error: {ex.Message}");
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    LogToTextBox($"Incorrect {qsoMessage.OriginalFormat} format, source {qsoMessage.Source}, data: {qsoMessage.OriginalQsoData}\r\n: {ex}\r\nIgnored");
                    continue;
                }
            }
        }

        private void SaveTemporarelyToDb(QsoMessage qsoMessage, string logMessage)
        {
            LogToTextBox($"{logMessage}\r\nTemporarely saving to local Database");
            List<Dictionary<string, string>> qsoRecords = ParseMessage(qsoMessage);
            dbRepository.SaveQsoRecords(qsoRecords, isImported: false, isTemporary: true);
            LogToTextBox($"Saved to local Database");
        }

        private List<Dictionary<string, string>> ParseMessage(QsoMessage qsoMessage)
        {
            List<Dictionary<string, string>> qsoRecords;
            try
            {
                string sourceIpAddress = IPAddress.Loopback.ToString();
                qsoRecords = qsoMessage.OriginalFormat switch
                {
                    "ADIF" => AdifToTableFieldsMapper.Map(qsoMessage, sourceIpAddress),
                    "N1MM" => N1mmContactInfoToTableFieldsMapper.Map(qsoMessage, sourceIpAddress),
                    _ => throw new ArgumentException($"Unsupported message format: {qsoMessage.OriginalFormat}"),
                };

                foreach (var record in qsoRecords)
                {
                    StringBuilder logMessage = new();
                    foreach (var kv in record)
                    {
                        logMessage.AppendLine($"{kv.Key}: {kv.Value})");
                    }
                    logMessage.Append("----");
                    LogToTextBox(logMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                LogToTextBox($"QSO message parsing error: {ex.Message}");
                throw new ArgumentException("QSO message parsing error", ex);
            }
            return qsoRecords;
        }

        private void LogToTextBox(String message)
        {
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText($"{message}\r\n");
            });
        }
    }
}