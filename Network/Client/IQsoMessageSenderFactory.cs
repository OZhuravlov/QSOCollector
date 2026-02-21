using System.Collections.Concurrent;
using System.Threading;
using QSOCollector.Data;
using QSOCollector.Helpers;

namespace QSOCollector.Network.Client
{
    public interface IQsoMessageSenderFactory
    {
        QsoMessageSender Create(string serverIp, int serverPort, BlockingCollection<QSOCollector.Models.QsoMessage> qsoMessageQueue, DbRepository dbRepository, ClientProgressUpdater progressUpdater, CancellationTokenSource cts);
    }
}
