namespace Warehouse.Application.Queries.GetCacheStatistics;

using MediatR;
using Warehouse.Domain.Caching;

public class GetCacheStatisticsHandler : IRequestHandler<GetCacheStatisticsQuery, GetCacheStatisticsResponse>
{
    private readonly ICacheService _cache;
    public GetCacheStatisticsHandler(ICacheService cache) => _cache = cache;

    public async Task<GetCacheStatisticsResponse> Handle(GetCacheStatisticsQuery request, CancellationToken cancellationToken)
    {
        var keys = await _cache.GetKeysAsync(cancellationToken);
        var stats = _cache.GetStatistics();

        return new GetCacheStatisticsResponse(keys, stats.HitCount, stats.MissCount, stats.LastRefreshedAt);
    }
}