namespace QSOCollector.Models
{
    public class ServerQsoAmount
    {
        public required string QsoAmountMode { get; set; }
        public required int TodayQsoAmount { get; set; }
        public required int TotalQsoAmount { get; set; }
        public required int ExportedQsoAmount { get; set; }
        public required DateTime LastQsoTime { get; set; }
        public DateTime LastExportedQsoTime { get; set; }
    }
}
