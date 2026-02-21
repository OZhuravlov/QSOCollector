namespace QSOCollector.Network.Client
{
    public class TimeoutOptions
    {
        public int ConnectTimeoutMs { get; set; } = 5000;
        public int WriteTimeoutMs { get; set; } = 2000;
        public int ResponseTimeoutMs { get; set; } = 5000;
    }
}
