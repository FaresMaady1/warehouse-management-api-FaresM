namespace Warehouse.Notifications.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Domain.Notifications;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;
    public NotificationRepository(NotificationsDbContext context) => _context = context;

    public Task<bool> ExistsForEventAsync(Guid sourceEventId, CancellationToken cancellationToken = default) =>
        _context.Notifications.AnyAsync(n => n.SourceEventId == sourceEventId, cancellationToken);

    public Task<List<Notification>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Notifications.OrderByDescending(n => n.CreatedAt).ToListAsync(cancellationToken);

    public Task<int> CountUnreadAsync(CancellationToken cancellationToken = default) =>
        _context.Notifications.CountAsync(n => n.Status == NotificationStatus.Unread, cancellationToken);

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public void Add(Notification notification) => _context.Notifications.Add(notification);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
