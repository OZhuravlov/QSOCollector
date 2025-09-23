using System.ComponentModel.DataAnnotations.Schema;


namespace QSOCollector
{
    public class ListenerConfig
    {
        public required int Id { get; set; }
        public required string Protocol { get; set; }
        public required int QsoPort { get; set; }
        public required int AcknowledgePort { get; set; }
        public required string MessageFormat { get; set; }
        public required bool IsActive { get; set; }
        public string? Description { get; set; }
    }

}
