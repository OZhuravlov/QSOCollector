using System.Xml.Serialization;

namespace QSOCollector
{

    [XmlRoot("contactinfo")]
    public class N1mmContactInfo
    {
        [XmlElement("app")]
        public string? App { get; set; }

        [XmlElement("contestname")]
        public string? ContestName { get; set; }

        [XmlElement("contestnr")]
        public int? ContestNr { get; set; }

        [XmlElement("timestamp")]
        public String TimestampStr
        {
            get { return this.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { this.Timestamp = DateTime.Parse(value); }
        }

        [XmlIgnore]
        public DateTime Timestamp { get; set; }

        [XmlElement("mycall")]
        public required string MyCall { get; set; }

        [XmlElement("band")]
        public required string Band { get; set; }

        [XmlElement("rxfreq")]
        public int? RxFreq { get; set; }

        [XmlElement("txfreq")]
        public int? TxFreq { get; set; }

        [XmlElement("operator")]
        public string? Operator { get; set; }

        [XmlElement("mode")]
        public required string Mode { get; set; }

        [XmlElement("call")]
        public required string Call { get; set; }

        [XmlElement("countryprefix")]
        public string? CountryPrefix { get; set; }

        [XmlElement("wpxprefix")]
        public string? WpxPrefix { get; set; }

        [XmlElement("stationprefix")]
        public string? StationPrefix { get; set; }

        [XmlElement("continent")]
        public string? Continent { get; set; }

        [XmlElement("snt")]
        public string? Snt { get; set; }

        [XmlElement("sntnr")]
        public string? SntNr { get; set; }

        [XmlElement("rcv")]
        public string? Rcv { get; set; }

        [XmlElement("rcvnr")]
        public string? RcvNr { get; set; }

        [XmlElement("gridsquare")]
        public string? Gridsquare { get; set; }

        [XmlElement("exchangel")]
        public string? Exchangel { get; set; }

        [XmlElement("exchange1")]
        public string? Exchange1 { get; set; }

        [XmlElement("section")]
        public string? Section { get; set; }

        [XmlElement("comment")]
        public string? Comment { get; set; }

        [XmlElement("qth")]
        public string? Qth { get; set; }

        [XmlElement("name")]
        public string? Name { get; set; }

        [XmlElement("power")]
        public string? Power { get; set; }

        [XmlElement("misctext")]
        public string? MiscText { get; set; }

        [XmlElement("zone")]
        public string? Zone { get; set; }

        [XmlElement("prec")]
        public string? Prec { get; set; }

        [XmlElement("ck")]
        public string? Ck { get; set; }

        [XmlElement("ismultiplierl")]
        public string? IsMultiplierl { get; set; }

        [XmlElement("ismultiplier1")]
        public string? IsMultiplier1 { get; set; }

        [XmlElement("ismultiplier2")]
        public string? IsMultiplier2 { get; set; }

        [XmlElement("ismultiplier3")]
        public string? IsMultiplier3 { get; set; }

        [XmlElement("points")]
        public int? Points { get; set; }

        [XmlElement("radionr")]
        public string? RadioNr { get; set; }

        [XmlElement("run1run2")]
        public string? Run1Run2 { get; set; }

        [XmlElement("RoverLocation")]
        public string? RoverLocation { get; set; }

        [XmlElement("RadioInterfaced")]
        public string? RadioInterfaced { get; set; }

        [XmlElement("NetworkedCompNr")]
        public string? NetworkedCompNr { get; set; }

        [XmlElement("IsOriginal")]
        public string? IsOriginal { get; set; }

        [XmlElement("NetBiosName")]
        public string? NetBiosName { get; set; }

        [XmlElement("IsRunQSO")]
        public string? IsRunQSO { get; set; }

        [XmlElement("StationName")]
        public string? StationName { get; set; }

        [XmlElement("ID")]
        public string? Id { get; set; }

        [XmlElement("IsClaimedQso")]
        public string? IsClaimedQso { get; set; }

        [XmlElement("oldtimestamp")]
        public String? OldTimestampStr
        {
            get { return this.OldTimestamp?.ToString("yyyy-MM-dd HH:mm:ss"); }

            set => this.OldTimestamp = value == null ? null : DateTime.Parse(value);
        }

        [XmlIgnore]
        public DateTime? OldTimestamp { get; set; }

        [XmlElement("oldcall")]
        public string? OldCall { get; set; }

        [XmlElement("SentExchange")]
        public string? SentExchange { get; set; }
    }
}
