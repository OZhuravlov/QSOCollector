namespace QSOCollector.Models
{
    public class StartupParams
    {
        public bool IsQuiet { get; set; } = false;
        public bool StartServer { get; set; } = false;
        public bool StartClient { get; set; } = false;
    }
}
