using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace QSOCollector
{
    public class UdpClientListener(ListenerConfig listenerConfig, string serverIp, int serverPort, DbRepository dbRepository, TextBox logTextBox)
    {
        const int responseTimeoutMs = 10000;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int localPort = listenerConfig.QsoPort;
            using UdpClient udpClient = new(localPort);
            LogToTextBox($"UDP Port {localPort} Listener started");
            TcpClientInstance tcpClient = new(serverIp, serverPort);

            bool continueReceiving = true;
            while (continueReceiving)
            {
                string receivedData = string.Empty;
                try
                {
                    var receivedResults = await udpClient.ReceiveAsync(cancellationToken);
                    byte[] receivedBytes = receivedResults.Buffer;
                    receivedData = Encoding.UTF8.GetString(receivedBytes);

                    QsoMessage qsoMessage = new() { OriginalFormat = listenerConfig.MessageFormat, AdifData = receivedData };
                    LogToTextBox(receivedData);

                    string qsoMessageJson = JsonSerializer.Serialize<QsoMessage>(qsoMessage);
                    try
                    {
                        if (!tcpClient.IsConnected())
                        {
                            tcpClient = new(serverIp, serverPort);
                        }
                        await tcpClient.SendMessage(qsoMessageJson, logTextBox, 10000);
                    }
                    catch (OperationCanceledException ex)
                    {
                        SaveTemporarelyToDb(qsoMessage, $"Error sending to server: {ex.Message}");
                    }
                    catch (SocketException ex)
                    {
                        SaveTemporarelyToDb(qsoMessage, $"Socket Connection error: {ex.Message}");
                    }
                }
                catch (ArgumentException ex)
                {
                    LogToTextBox($"Incorrect {listenerConfig.MessageFormat} format data {receivedData}\r\n: {ex.Message}\r\nIgnored");
                    continue;
                }
                catch (OperationCanceledException)
                {
                    udpClient.Close();
                    udpClient.Dispose();
                    LogToTextBox($"UDP Port {localPort} Listener stopped");
                }
            }
        }

        private void SaveTemporarelyToDb(QsoMessage qsoMessage, string logMessage)
        {
            LogToTextBox($"{logMessage}\r\nTemporarely saving to local Database");
            List<Dictionary<string, string>> qsoRecords = ParseMessage(qsoMessage, logTextBox);
            dbRepository.SaveQsoRecords(qsoRecords, isImported: false, isTemporary: true);
            LogToTextBox($"Saved to local Database");
        }

        private List<Dictionary<string, string>> ParseMessage(QsoMessage qsoMessage, TextBox logTextBox)
        {
            List<Dictionary<string, string>> qsoRecords;
            try
            {
                if (qsoMessage.OriginalFormat == "ADIF")
                {
                    qsoRecords = AdifParser.Parse(qsoMessage, IPAddress.Loopback.ToString());
                }
                else
                {
                    throw new ArgumentException($"Unsupported message format: {qsoMessage.OriginalFormat}");
                }

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