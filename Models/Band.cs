namespace QSOCollector.Models
{
    public class Band
    {
        public int Id { get; set; }
        public string BandName { get; set; }
        public string AltBandName { get; set; }
        public string N1mmBandName { get; set; }
        public string FullBandName { get; set; }
        public double FreqFrom { get; set; }
        public double FreqTo { get; set; }
        public string Designator { get; set; }
        public bool IsActive { get; set; }
    }
}
