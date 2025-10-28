namespace QSOCollector.Helpers
{
    public class ClientProgressUpdater(
            Label receivedLabel,
            Label receivedAtLabel,
            Label sentToServerLabel,
            Label sentToServerAtLabel,
            Label tempSavedLabel,
            Label tempSavedAtLabel,
            Label rejectedLabel,
            Label rejectedAtLabel,
            Label serverStatusLabel,
            Label serverStatusAtLabel,
            TextBox clientLog)
    {
        public bool? IsDebug { get; set; } = false;

        private int received;
        private int sentToServer;
        private int tempSaved;
        private int rejected;

        private readonly object receivedLock = new();
        private readonly object sentToServerLock = new();
        private readonly object tempSavedLock = new();
        private readonly object rejectedLock = new();
        private readonly object statusLock = new();

        public void UpdateProgress(
            bool isReceived,
            bool isSentToServer,
            bool isTempSaved,
            bool isRejected,
            string? logMessage = null)
        {
            string updateTs = DateTime.UtcNow.ToString("dd-MMM HH:mm");
            if (isReceived) UpdateReceived(updateTs);
            if (isSentToServer) UpdateSentToServer(updateTs);
            if (isTempSaved) UpdateTempSaved(updateTs);
            if (isRejected) UpdateRejected(updateTs);
            UpdateLog(logMessage);
        }

        public void UpdateServerStatus(string status, string? logMessage)
        {
            UpdateServerStatus(status);
            UpdateLog(logMessage);
        }

        public void UpdateLog(string? message, bool isDebug = false)
        {
            if (isDebug && !(IsDebug ?? false)) return;

            if (!string.IsNullOrEmpty(message))
            {
                lock (statusLock)
                {
                    clientLog.BeginInvoke((MethodInvoker)delegate
                    {
                        clientLog.AppendText($"{message}\r\n");
                    });
                }
            }
            ;
        }

        public void UpdateTempSaved(int amount)
        {
            lock (tempSavedLock)
            {
                tempSaved = Math.Max(0, tempSaved + amount);
                tempSavedLabel.BeginInvoke((MethodInvoker)delegate
                {
                    tempSavedLabel.Text = tempSaved.ToString();
                });
            }
        }

        private void UpdateReceived(string updateTs)
        {
            lock (receivedLock)
            {
                received++;
                receivedLabel.BeginInvoke((MethodInvoker)delegate
                {
                    receivedLabel.Text = received.ToString();
                    receivedAtLabel.Text = updateTs;
                });
            }
        }
        private void UpdateSentToServer(string updateTs)
        {
            lock (sentToServerLock)
            {
                sentToServer++;
                sentToServerLabel.BeginInvoke((MethodInvoker)delegate
                {
                    sentToServerLabel.Text = sentToServer.ToString();
                    sentToServerAtLabel.Text = updateTs;
                });
            }
        }
        private void UpdateTempSaved(string updateTs)
        {
            lock (tempSavedLock)
            {
                tempSaved++;
                tempSavedLabel.BeginInvoke((MethodInvoker)delegate
                {
                    tempSavedLabel.Text = tempSaved.ToString();
                    tempSavedAtLabel.Text = updateTs;
                });
            }
        }
        private void UpdateRejected(string updateTs)
        {
            lock (rejectedLock)
            {
                rejected++;
                rejectedLabel.BeginInvoke((MethodInvoker)delegate
                {
                    rejectedLabel.Text = rejected.ToString();
                    rejectedAtLabel.Text = updateTs;
                });
            }
        }
        private void UpdateServerStatus(string status)
        {
            lock (statusLock)
            {
                serverStatusLabel.BeginInvoke((MethodInvoker)delegate
                {
                    if (status == "Active")
                    {
                        serverStatusLabel.ForeColor = Color.Green;
                        if (serverStatusLabel.Text != "Active")
                        {
                            clientLog.AppendText($"Server is available now\r\n");
                        }
                    }
                    else if (status == "Unavailable")
                    {
                        serverStatusLabel.ForeColor = Color.Red;
                    }
                    else
                    {
                        serverStatusLabel.ForeColor = Color.Gray;
                    }
                    serverStatusLabel.Text = status;
                    serverStatusAtLabel.Text = DateTime.UtcNow.ToString("dd-MMM HH:mm");
                });
            }
        }
    }
}