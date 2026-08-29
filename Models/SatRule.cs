namespace QSOCollector.Models
{
    public class SatRule
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double SourceFreqFrom { get; set; }
        public double SourceFreqTo { get; set; }
        public string PropagationMode { get; set; }
        public string SatName { get; set; }
        public string? SatMode { get; set; }
        public int? BandTxId { get; set; }
        public Band? BandTx { get; set; }
        public int? BandRxId { get; set; }
        public Band? BandRx { get; set; }
        public double? FreqTx { get; set; }
        public double? FreqRx { get; set; }
        public bool IsActive { get; set; }
        public bool IsApplyForImport { get; set; }
    }
}
