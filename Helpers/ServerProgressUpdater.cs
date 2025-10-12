using System.Data;

namespace QSOCollector.Helpers
{
    public class ServerProgressUpdater(DataTable dataTable, TextBox serverLog)
    {
        public bool IsDebug { get; set; } = false;
        public object GridLock { get; } = new();

        private readonly DataTable dataTable = dataTable;
        private readonly TextBox serverLog = serverLog;
        private DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        public void UpdateProgress(DateTime qsoDateTime, string mode, string logMessage)
        {
            lock (GridLock)
            {
                DataRow row = dataTable.Rows.Find(mode);
                if (row == null)
                {
                    row = dataTable.NewRow();
                    row.ItemArray = [mode, 0, 0, 0, null];
                    dataTable.Rows.InsertAt(row, dataTable.Rows.Count - 1);
                }
                UpdateRow(row, qsoDateTime);
                UpdateRow(dataTable.Rows.Find("Total"), qsoDateTime);
            }
            UpdateLog(logMessage);
        }

        public void UpdateLog(string? message, bool isDebug = false)
        {
            if (isDebug && !IsDebug) return;

            if (!string.IsNullOrEmpty(message))
            {
                serverLog.Invoke((MethodInvoker)delegate
                {
                    serverLog.AppendText($"{message}\r\n");
                });
            }
            ;
        }

        public static DateTime ParseDateTime(string dateTimeString, string format = "yyyy-MM-dd HH:mm:ss")
        {
            DateTime.TryParseExact(dateTimeString, format[..dateTimeString.Length], null, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime qsoTime);
            return qsoTime;
        }

        public static string? ParseDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        private void UpdateRow(DataRow row, DateTime qsoDateTime)
        {
            row["TodayQsoAmount"] = (int)(long)row["TodayQsoAmount"] + 1;
            row["TotalQsoAmount"] = (int)(long)row["TotalQsoAmount"] + 1;

            DateTime lastQsoTime = ParseDateTime(row["LastQsoTime"].ToString());
            if (qsoDateTime > lastQsoTime)
            {
                row["LastQsoTime"] = ParseDateTime(qsoDateTime);
            }
            DateOnly qsoDate = DateOnly.FromDateTime(qsoDateTime);
            if (qsoDate > today)
            {
                today = qsoDate;
                foreach (DataRow r in dataTable.Rows)
                {
                    r["TodayQsoAmount"] = 0;
                }
                row["TodayQsoAmount"] = 1;
            }
        }
    }
}