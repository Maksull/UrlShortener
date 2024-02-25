using Core.Logging.Caching;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace UrlShortenerApi.Caching;

public sealed class RedisOutputCacheStore : IOutputCacheStore
{
    private IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private IConnectionMultiplexer? _connectionMultiplexer;
    private bool _isConnectionError;
    private DateTime _lastReconnectAttempt = DateTime.UtcNow;

    public RedisOutputCacheStore(IDistributedCache cache, IConfiguration configuration, ILogger<RedisOutputCacheStore> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        try
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("RedisCache")!);
        }
        catch (RedisConnectionException)
        {
            _connectionMultiplexer = null;
            _isConnectionError = true;
        }
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            if (IsReconnect())
            {
                await Reconnect();
            }
            if (!_isConnectionError)
            {
                var value = await _cache.GetAsync(key, cancellationToken);

                return value;
            }

            return null;
        }
        catch (RedisConnectionException e)
        {
            _isConnectionError = true;
            _logger.LogRedisConnectionFailed(e.ToString());

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
            if (!_isConnectionError)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(validFor);

                await _cache.SetAsync(key, value, cacheEntryOptions, cancellationToken);
            }
        }
        catch (RedisConnectionException e)
        {
            _isConnectionError = true;
            _logger.LogRedisConnectionFailed(e.ToString());
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
            if (!_isConnectionError)
            {
                var db = _connectionMultiplexer!.GetDatabase();
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
            _logger.LogRedisConnectionFailed(e.ToString());
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

            if (!redis.IsConnected) return;

            _cache = new RedisCache(new RedisCacheOptions
            {
                Configuration = redisConnectionString,
                InstanceName = "Redis"
            });
            _connectionMultiplexer = redis;

            _isConnectionError = false;
        }
        catch (RedisConnectionException e)
        {
            _logger.LogRedisConnectionFailed(e.ToString());
        }
    }
}
