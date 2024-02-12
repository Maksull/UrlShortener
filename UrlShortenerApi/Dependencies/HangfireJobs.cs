using Hangfire;
using Infrastructure.BackgroundServices.Interfaces;

namespace UrlShortenerApi.Dependencies;

public static class HangfireJobs
{
    public static void AddHangfireJobs(this WebApplication webApplication)
    {
        RecurringJob.AddOrUpdate<IDeleteService>(
            "delete-urls",
            s => s.DeleteOldUrls(),
            Cron.Minutely(),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc,
            }
        );
    }
}
