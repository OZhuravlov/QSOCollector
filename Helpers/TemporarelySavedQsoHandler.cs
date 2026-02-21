using QSOCollector.Data;
using QSOCollector.Models;
using QSOCollector.Network.Client;
using System.Collections.Concurrent;

namespace QSOCollector.Helpers
{
    public class TemporarelySavedQsoHandler(DbRepository dbRepository, BlockingCollection<QsoMessage> qsoMessageQueue,
        QsoMessageSender qsoMessageSender, ClientProgressUpdater progressUpdater, CancellationTokenSource cancellationTokenSource)
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
                    if (qsoMessageSender.IsConnected() || tryNumber >= 5)
                    {
                        tryNumber = 0;
                        var tempQsoMessages = dbRepository.GetTemporaryQsoMessages();
                        foreach (var qsoMessage in tempQsoMessages)
                        {
                            qsoMessageQueue.Add(qsoMessage.Value);
                            dbRepository.DeleteTemporaryQsoRecord(qsoMessage.Key);
                            progressUpdater.UpdateLog($"Resend to server locally saved QSO id {qsoMessage.Key} from {qsoMessage.Value.Source}");
                        }
                        progressUpdater.UpdateTempSaved(-tempQsoMessages.Count);
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    cancellationTokenSource.Dispose();
                    qsoMessageQueue.CompleteAdding();
                    progressUpdater.UpdateLog("Saved Qso resender stopped");
                    break;
                }
                catch (Exception ex)
                {
                    progressUpdater.UpdateLog($"Error when try to put temporarely saved QSOs for reprocessing: {ex}");
                    break;
                }
            }
        }
    }
}