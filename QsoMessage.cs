namespace QSOCollector
{
    public class QsoMessage
    {
        public string? Source { get; set; }
        public required string OriginalFormat { get; set; }
        public string OriginalQsoData { get; set; }
        public string AdifQsoData { get; set; }
    }
}
