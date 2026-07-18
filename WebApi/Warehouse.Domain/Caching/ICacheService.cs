namespace Warehouse.Domain.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<List<string>> GetKeysAsync(CancellationToken cancellationToken = default);
    CacheStatistics GetStatistics();
}

public record CacheStatistics(int HitCount, int MissCount, DateTime? LastRefreshedAt);