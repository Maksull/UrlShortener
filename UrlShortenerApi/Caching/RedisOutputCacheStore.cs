using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace UrlShortenerApi.Caching;

public sealed class RedisOutputCacheStore : IOutputCacheStore
{
    private IDistributedCache _cache;
    private IConfiguration _configuration;
    private readonly ILogger<RedisOutputCacheStore> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private bool _isConnectionError;
    private DateTime _lastReconnectAttempt = DateTime.UtcNow;

    public RedisOutputCacheStore(IDistributedCache cache, IConfiguration configuration, ILogger<RedisOutputCacheStore> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            if (IsReconnect())
            {
                await Reconnect();
            }
            else if (!_isConnectionError)
            {
                var value = await _cache.GetAsync(key, cancellationToken);

                return value;
            }

            return null;
        }
        catch (RedisConnectionException e)
        {
            _isConnectionError = true;
            _logger.LogCritical("Redis connection failed: {RedisConnectionError}", e.ToString());

            return null;
        }
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        try
        {
            if (IsReconnect())
            {
                await Reconnect();
            }
            else if (!_isConnectionError)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(validFor);

                await _cache.SetAsync(key, value, cacheEntryOptions, cancellationToken);
            }
        }
        catch (RedisConnectionException e)
        {
            _isConnectionError = true;
            _logger.LogCritical("Redis connection failed: {RedisConnectionError}", e.ToString());
        }
    }

    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        try
        {
            if (IsReconnect())
            {
                await Reconnect();
            }
            else if (!_isConnectionError)
            {
                await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("RedisCache")!);

                var db = _connectionMultiplexer.GetDatabase();
                var cachedKeys = await db.SetMembersAsync(tag);

                var keys = cachedKeys
                    .Select(k => (RedisKey)k.ToString())
                    .Concat([(RedisKey)tag])
                    .ToArray();

                await db.KeyDeleteAsync(keys);
            }
        }
        catch (RedisConnectionException e)
        {
            _isConnectionError = true;
            _logger.LogCritical("Redis connection failed: {RedisConnectionError}", e.ToString());
        }
    }

    private bool IsReconnect()
    {
        const double minutesBeforeReconnect = 2;

        return _isConnectionError && (DateTime.UtcNow - _lastReconnectAttempt).TotalMinutes >= minutesBeforeReconnect;
    }

    private async Task Reconnect()
    {
        try
        {
            _lastReconnectAttempt = DateTime.UtcNow;
            var redisConnectionString = _configuration.GetConnectionString("RedisCache")!;
            var redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);

            if (redis.IsConnected)
            {
                _cache = new RedisCache(new RedisCacheOptions
                {
                    Configuration = redisConnectionString,
                    InstanceName = "Redis"
                });

                _isConnectionError = false;
            }
        }
        catch (RedisConnectionException e)
        {
            _logger.LogCritical("Redis reconnect failed: {RedisReconnectError}", e.ToString());
        }
    }
}
