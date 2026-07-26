namespace Warehouse.Notifications.Application.Queries.ListNotifications;

public record NotificationResponse(
    Guid Id,
    string Type,
    string Severity,
    string Title,
    string Message,
    string RelatedEntityId,
    string RelatedEntityType,
    string Status,
    DateTime CreatedAt,
    DateTime? ReadAt);
