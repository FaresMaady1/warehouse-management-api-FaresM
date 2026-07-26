namespace Warehouse.Notifications.Application.Commands.CreateStockLowNotification;

using MediatR;
using Warehouse.Notifications.Application.Events;

public record CreateStockLowNotificationCommand(StockLowDetectedEvent Event) : IRequest<bool>;
