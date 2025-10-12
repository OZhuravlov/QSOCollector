using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QSOCollector.Models
{
    public class ServerResponse(ServerResponseStatus status, string? errorDescription = null)
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ServerResponseStatus Status { get; set; } = status;
        public string? ErrorDescription { get; set; } = errorDescription;

        public override string ToString()
        {
            if (Status == ServerResponseStatus.Ok)
                return "OK";
            return $"{Status}: {ErrorDescription}";
        }
    }
}