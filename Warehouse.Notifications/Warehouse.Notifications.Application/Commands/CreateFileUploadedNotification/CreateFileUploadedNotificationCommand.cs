namespace Warehouse.Notifications.Application.Commands.CreateFileUploadedNotification;

using MediatR;
using Warehouse.Notifications.Application.Events;

public record CreateFileUploadedNotificationCommand(WarehouseFileUploadedEvent Event) : IRequest<bool>;
