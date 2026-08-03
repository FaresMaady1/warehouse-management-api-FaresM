namespace Warehouse.Notifications.Domain.Notifications;

public interface INotificationRepository
{
    Task<bool> ExistsForEventAsync(Guid sourceEventId, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountUnreadAsync(CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Notification notification);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
