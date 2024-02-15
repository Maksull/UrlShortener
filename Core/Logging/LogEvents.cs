namespace Core.Logging;

public static class LogEvents
{
    public const int DeletedExpiredUrlId = 1001;
    public const string DeletedExpiredUrlName = "UrlsWereDeleted";

    public const int RedisConnectionFailedId = 1002;
    public const string RedisConnectionFailedName = "RedisConnectionFailed";

    public const int UnhandledExceptionId = 1003;
    public const string LogUnhandledExceptionName = "UnhandledException";
}
