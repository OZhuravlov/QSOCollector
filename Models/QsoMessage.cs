namespace QSOCollector.Models
{
    public class QsoMessage
    {
        public string? Source { get; set; }
        public string OriginalFormat { get; set; }
        public string OriginalQsoData { get; set; }
        public string AdifQsoData { get; set; }

        public bool IsHeartBeat { get => "TEST".Equals(Source, StringComparison.OrdinalIgnoreCase); }
    }
}
