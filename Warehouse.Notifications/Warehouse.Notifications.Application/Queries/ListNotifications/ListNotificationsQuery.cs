namespace Warehouse.Notifications.Application.Queries.ListNotifications;

using MediatR;

public record ListNotificationsQuery() : IRequest<List<NotificationResponse>>;
