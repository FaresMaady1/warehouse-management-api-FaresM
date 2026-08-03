namespace Warehouse.Domain.Notifications;

public interface INotificationServiceClient
{
    Task<int?> GetUnreadCountAsync(CancellationToken cancellationToken = default);
}
