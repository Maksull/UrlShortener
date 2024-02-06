using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace UrlShortenerApi.Caching;

public static class RedisOutputCacheServiceCollectionExtensions
{
    public static IServiceCollection AddRedisOutputCache(this IServiceCollection services, Action<OutputCacheOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddOutputCache();

        services.RemoveAll<IOutputCacheStore>();
        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>();

        return services;
    }
}
