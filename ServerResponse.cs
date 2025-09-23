namespace QSOCollector
{
    public class ServerResponse
    {
        public ServerResponseStatus Status { get; set; }
        public string? ErrorDescription { get; set; }

        public ServerResponse(ServerResponseStatus status, string? errorDescription = null)
        {
            Status = status;
            ErrorDescription = errorDescription;
        }

        public override string ToString()
        {
            if (Status == ServerResponseStatus.Ok)
                return "OK";
            return $"{Status}: {ErrorDescription}";
        }
    }
}