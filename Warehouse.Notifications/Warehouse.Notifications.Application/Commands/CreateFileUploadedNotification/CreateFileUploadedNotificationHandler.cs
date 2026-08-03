namespace Warehouse.Notifications.Application.Commands.CreateFileUploadedNotification;

using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Notifications.Application.Preferences;
using Warehouse.Notifications.Domain.Notifications;

public class CreateFileUploadedNotificationHandler : IRequestHandler<CreateFileUploadedNotificationCommand, bool>
{
    private readonly INotificationRepository _repository;
    private readonly NotificationPreferences _preferences;

    public CreateFileUploadedNotificationHandler(INotificationRepository repository, IOptions<NotificationPreferences> preferences)
    {
        _repository = repository;
        _preferences = preferences.Value;
    }

    public async Task<bool> Handle(CreateFileUploadedNotificationCommand request, CancellationToken cancellationToken)
    {
        var evt = request.Event;

        if (await _repository.ExistsForEventAsync(evt.EventId, cancellationToken))
            return false;

        var notification = Notification.Create(
            evt.EventId,
            "FileUploaded",
            _preferences.FileUploadedSeverity,
            "New file uploaded",
            $"{evt.FileName} was uploaded for {evt.EntityType} {evt.EntityId}.",
            evt.EntityId,
            evt.EntityType);

        _repository.Add(notification);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
