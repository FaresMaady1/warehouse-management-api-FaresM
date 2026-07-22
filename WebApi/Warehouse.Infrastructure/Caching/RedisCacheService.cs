namespace Warehouse.Infrastructure.Caching;

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Warehouse.Domain.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly string _instanceName;

    private static int _hitCount;
    private static int _missCount;
    private static DateTime? _lastRefreshedAt;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer, string instanceName)
    {
        _cache = cache;
        _connectionMultiplexer = connectionMultiplexer;
        _instanceName = instanceName;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        if (json == null)
        {
            Interlocked.Increment(ref _missCount);
            return default;
        }

        Interlocked.Increment(ref _hitCount);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        }, cancellationToken);

        _lastRefreshedAt = DateTime.Now;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    public Task<List<string>> GetKeysAsync(CancellationToken cancellationToken = default)
    {
        var endpoint = _connectionMultiplexer.GetEndPoints().First();
        var server = _connectionMultiplexer.GetServer(endpoint);

        var keys = server.Keys(pattern: $"{_instanceName}*")
            .Select(k => k.ToString().Replace(_instanceName, string.Empty))
            .ToList();

        return Task.FromResult(keys);
    }

    public CacheStatistics GetStatistics() => new(_hitCount, _missCount, _lastRefreshedAt);
}