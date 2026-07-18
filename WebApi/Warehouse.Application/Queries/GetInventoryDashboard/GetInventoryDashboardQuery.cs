namespace Warehouse.Application.Queries.GetInventoryDashboard;

using MediatR;

public record GetInventoryDashboardQuery() : IRequest<GetInventoryDashboardResponse>;