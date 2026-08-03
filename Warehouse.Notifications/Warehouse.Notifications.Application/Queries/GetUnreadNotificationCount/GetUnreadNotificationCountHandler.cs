namespace Warehouse.Notifications.Application.Queries.GetUnreadNotificationCount;

using MediatR;
using Warehouse.Notifications.Domain.Notifications;

public class GetUnreadNotificationCountHandler : IRequestHandler<GetUnreadNotificationCountQuery, GetUnreadNotificationCountResponse>
{
    private readonly INotificationRepository _repository;
    public GetUnreadNotificationCountHandler(INotificationRepository repository) => _repository = repository;

    public async Task<GetUnreadNotificationCountResponse> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _repository.CountUnreadAsync(cancellationToken);
        return new GetUnreadNotificationCountResponse(count);
    }
}
