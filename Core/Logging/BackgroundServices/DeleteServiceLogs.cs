using Microsoft.Extensions.Logging;

namespace Core.Logging.BackgroundServices;

public static partial class DeleteServiceLogs
{
    [LoggerMessage(EventId = LogEvents.DeletedExpiredUrlId, EventName = LogEvents.DeletedExpiredUrlName,
        Level = LogLevel.Information,
        Message = "Background service removed {DeletedUrlsCount} urls because of the expiration date",
        SkipEnabledCheck = true)]
    public static partial void LogDeletedExpiredUrl(this ILogger logger, int deletedUrlsCount);
}
