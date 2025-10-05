namespace QSOCollector
{
    public class QsoExportFilters : ICloneable
    {
        public string? ProgramId { get; set; }
        public bool? IsNewOnly { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? ModeGroup { get; set; }
        public string? Mode { get; set; }
        public string? Band { get; set; }
        public string? Operator { get; set; }
        public string? SourceIp { get; set; }

        #region ICloneable Members
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
