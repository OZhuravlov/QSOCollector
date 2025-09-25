using System.Collections.Concurrent;

namespace QSOCollector
{
    public class TemporarelySavedQsoHandler(DbRepository dbRepository, BlockingCollection<QsoMessage> qsoMessageQueue,
        QsoMessageSender qsoMessageSender, TextBox logTextBox, CancellationTokenSource cancellationTokenSource)
    {

        public async Task Start()
        {
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            int tryNumber = 0;
            while (true)
            {
                try
                {
                    tryNumber++;
                    if (qsoMessageSender.IsConnected() || tryNumber >= 5) {
                        tryNumber = 0;
                        var tempQsoMessages = dbRepository.GetTemporaryQsoMessages();
                        foreach (var qsoMessage in tempQsoMessages)
                        {
                            qsoMessageQueue.Add(qsoMessage.Value);
                            dbRepository.DeleteTemporaryQsoRecord(qsoMessage.Key);
                            LogToTextBox($"Put QSO id {qsoMessage.Key} to be resent:{qsoMessage.Value.QsoData}");
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    cancellationTokenSource.Dispose();
                    qsoMessageQueue.CompleteAdding();
                    LogToTextBox("TemporarelySavedQsoResender stopped");
                    break;
                }
                catch (Exception ex)
                {
                    LogToTextBox($"Error when try to put temporarely saved QSOs for reprocessing: {ex}");
                    break;
                }
            }
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