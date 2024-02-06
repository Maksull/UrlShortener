using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;

namespace UrlShortenerApi.Caching;

public sealed class RedisOutputCacheStore : IOutputCacheStore
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisOutputCacheStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);

        var db = _connectionMultiplexer.GetDatabase();

        return await db.StringGetAsync(key);
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var db = _connectionMultiplexer.GetDatabase();

        foreach (var tag in tags ?? [])
        {
            await db.SetAddAsync(tag, key);
        }

        await db.StringSetAsync(key, value, validFor);

    }

    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tag);

        var db = _connectionMultiplexer.GetDatabase();
        var cachedKeys = await db.SetMembersAsync(tag);

        var keys = cachedKeys
            .Select(k => (RedisKey)k.ToString())
            .Concat([(RedisKey)tag])
            .ToArray();

        await db.KeyDeleteAsync(keys);
    }
}
