namespace QSOCollector
{
    public class ListenerConfig
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int QsoPort { get; set; }
        public int? ForwardPort { get; set; }
        public int? AcknowledgePort { get; set; }
        public required string MessageFormat { get; set; }
        public required bool IsActive { get; set; }
    }

}
