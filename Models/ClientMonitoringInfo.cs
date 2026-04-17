namespace QSOCollector.Models
{
    public class ClientMonitoringInfo
    {
        public required string IpAddress { get; set; }
        public ClientStatus Status { get; set; }
        public DateTime ConnectionTime { get; set; }
        public DateTime LastActivityTime { get; set; }
        public int QsosReceived { get; set; }
    }

    public enum ClientStatus
    {
        Unknown,
        Connected,
        Disconnected
    }
}
