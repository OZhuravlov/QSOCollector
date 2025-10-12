namespace QSOCollector.Models
{
    public class QsoExportExpectedAmounts
    {
        public string SourceName { get; set; }
        public bool IsExported { get; set; }
        public DateTime QsoDate { get; set; }
        public string ModeGroup { get; set; }
        public string Mode { get; set; }
        public string Band { get; set; }
        public string Operator { get; set; }
        public string SourceIp { get; set; }
        public int Count { get; set; }
    }
}
