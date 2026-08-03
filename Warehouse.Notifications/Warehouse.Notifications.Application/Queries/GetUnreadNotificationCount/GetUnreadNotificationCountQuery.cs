namespace Warehouse.Notifications.Application.Queries.GetUnreadNotificationCount;

using MediatR;

public record GetUnreadNotificationCountQuery() : IRequest<GetUnreadNotificationCountResponse>;
