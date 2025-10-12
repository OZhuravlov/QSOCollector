namespace QSOCollector.Models
{
    public enum ServerResponseStatus
    {
        Ok,
        SqliteError,
        ArgumentError,
        UnknownError
    }

    public static class ServerResponseStatusExtensions
    {
        public static bool IsErrorStatus(this ServerResponseStatus status) =>
            status != ServerResponseStatus.Ok;

        public static bool IsRetryable(this ServerResponseStatus status) =>
            status == ServerResponseStatus.SqliteError;
    }
}