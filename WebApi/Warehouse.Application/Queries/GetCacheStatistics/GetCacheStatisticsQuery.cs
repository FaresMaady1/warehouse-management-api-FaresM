namespace Warehouse.Application.Queries.GetCacheStatistics;

using MediatR;

public record GetCacheStatisticsQuery() : IRequest<GetCacheStatisticsResponse>;