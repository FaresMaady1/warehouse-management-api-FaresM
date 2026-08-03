namespace Warehouse.Notifications.Application.Commands.CreateStockLowNotification;

using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Notifications.Application.Preferences;
using Warehouse.Notifications.Domain.Notifications;

public class CreateStockLowNotificationHandler : IRequestHandler<CreateStockLowNotificationCommand, bool>
{
    private readonly INotificationRepository _repository;
    private readonly NotificationPreferences _preferences;

    public CreateStockLowNotificationHandler(INotificationRepository repository, IOptions<NotificationPreferences> preferences)
    {
        _repository = repository;
        _preferences = preferences.Value;
    }

    public async Task<bool> Handle(CreateStockLowNotificationCommand request, CancellationToken cancellationToken)
    {
        var evt = request.Event;

        if (await _repository.ExistsForEventAsync(evt.EventId, cancellationToken))
            return false; // already processed this exact event - keeps the consumer idempotent

        var notification = Notification.Create(
            evt.EventId,
            "StockLow",
            _preferences.StockLowSeverity,
            $"Low stock: {evt.ProductName}",
            $"{evt.ProductName} ({evt.Sku}) is down to {evt.CurrentQuantity} units, below the threshold of {evt.Threshold}.",
            evt.ProductId,
            "Product");

        _repository.Add(notification);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
