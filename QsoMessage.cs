namespace QSOCollector
{
    public class QsoMessage
    {
        public string? Source { get; set; }
        public required string OriginalFormat { get; set; }
        public required string QsoData { get; set; }
    }
}
