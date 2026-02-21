using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Options;
using QSOCollector.Data;
using QSOCollector.Helpers;

namespace QSOCollector.Network.Client
{
    public class QsoMessageSenderFactory : IQsoMessageSenderFactory
    {
        private readonly TimeoutOptions options;

        public QsoMessageSenderFactory(IOptions<TimeoutOptions> options)
        {
            this.options = options?.Value ?? new TimeoutOptions();
        }

        public QsoMessageSender Create(string serverIp, int serverPort, BlockingCollection<QSOCollector.Models.QsoMessage> qsoMessageQueue, DbRepository dbRepository, ClientProgressUpdater progressUpdater, CancellationTokenSource cts)
        {
            return new QsoMessageSender(serverIp, serverPort, qsoMessageQueue, dbRepository, progressUpdater, cts, options);
        }
    }
}
