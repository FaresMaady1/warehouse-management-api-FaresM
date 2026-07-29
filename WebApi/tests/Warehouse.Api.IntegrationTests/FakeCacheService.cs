namespace Warehouse.Api.IntegrationTests;

using System.Collections.Concurrent;
using Warehouse.Domain.Caching;

public class FakeCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    // GetAsync
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.TryGetValue(key, out var value) ? (T?)value : default);

    // SetAsync
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        _store[key] = value!;
        return Task.CompletedTask;
    }

    // RemoveAsync
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    // GetKeysAsync
    public Task<List<string>> GetKeysAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.Keys.ToList());

    // GetStatistics
    public CacheStatistics GetStatistics() => new(0, 0, null);
}