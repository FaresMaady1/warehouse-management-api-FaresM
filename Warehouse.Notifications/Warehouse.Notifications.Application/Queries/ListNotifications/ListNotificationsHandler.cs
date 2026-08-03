namespace Warehouse.Notifications.Application.Queries.ListNotifications;

using MediatR;
using Warehouse.Notifications.Domain.Notifications;

public class ListNotificationsHandler : IRequestHandler<ListNotificationsQuery, List<NotificationResponse>>
{
    private readonly INotificationRepository _repository;
    public ListNotificationsHandler(INotificationRepository repository) => _repository = repository;

    public async Task<List<NotificationResponse>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetAllAsync(cancellationToken);

        return notifications
            .Select(n => new NotificationResponse(
                n.Id, n.Type, n.Severity, n.Title, n.Message,
                n.RelatedEntityId, n.RelatedEntityType, n.Status.ToString(), n.CreatedAt, n.ReadAt))
            .ToList();
    }
}
