namespace Warehouse.Notifications.Application.Commands.MarkNotificationAsRead;

using MediatR;
using Warehouse.Notifications.Domain.Notifications;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationRepository _repository;
    public MarkNotificationAsReadHandler(INotificationRepository repository) => _repository = repository;

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null) return false;

        notification.MarkAsRead();
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
