namespace Warehouse.Notifications.Application.Events;

public record WarehouseFileUploadedEvent(
    Guid EventId,
    DateTime OccurredAt,
    string CorrelationId,
    string EntityId,
    string EntityType,
    string FileName,
    string ContentType,
    long SizeBytes);
