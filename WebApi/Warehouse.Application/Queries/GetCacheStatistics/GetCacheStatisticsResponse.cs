namespace Warehouse.Application.Queries.GetCacheStatistics;

public record GetCacheStatisticsResponse(
    List<string> CachedKeys,
    int CacheHitCount,
    int CacheMissCount,
    DateTime? LastCacheRefreshedAt);