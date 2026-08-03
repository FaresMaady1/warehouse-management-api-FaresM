namespace Warehouse.Notifications.Application.Commands.MarkNotificationAsRead;

using MediatR;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<bool>;
