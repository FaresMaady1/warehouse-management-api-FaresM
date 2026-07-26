namespace Warehouse.Notifications.Application.Events;

public record StockLowDetectedEvent(
    Guid EventId,
    DateTime OccurredAt,
    string CorrelationId,
    string ProductId,
    string ProductName,
    string Sku,
    int CurrentQuantity,
    int Threshold);
