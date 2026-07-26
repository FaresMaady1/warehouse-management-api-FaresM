namespace Warehouse.Notifications.Domain.Notifications;

public enum NotificationStatus
{
    Unread,
    Read,
    Failed
}

public class Notification
{
    public Guid Id { get; private set; }

    public Guid SourceEventId { get; private set; }

    public string Type { get; private set; } = default!;
    public string Severity { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string RelatedEntityId { get; private set; } = default!;
    public string RelatedEntityType { get; private set; } = default!;
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid sourceEventId, string type, string severity, string title, string message,
        string relatedEntityId, string relatedEntityType)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            SourceEventId = sourceEventId,
            Type = type,
            Severity = severity,
            Title = title,
            Message = message,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            Status = NotificationStatus.Unread,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }
}
