using Microsoft.Extensions.Logging;

namespace Core.Logging.ExceptionHandler;

public static partial class GlobalExceptionHandlerLogs
{
    [LoggerMessage(EventId = LogEvents.UnhandledExceptionId, EventName = LogEvents.LogUnhandledExceptionName,
       Level = LogLevel.Error,
       Message = "An unhandled exception occurred. Exception: {UnhandledException}",
       SkipEnabledCheck = true)]
    public static partial void LogUnhandledException(this ILogger logger, string unhandledException);
}
