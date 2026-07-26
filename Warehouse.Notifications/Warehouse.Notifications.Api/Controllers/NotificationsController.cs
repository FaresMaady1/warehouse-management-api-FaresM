namespace Warehouse.Notifications.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Notifications.Application.Commands.MarkNotificationAsRead;
using Warehouse.Notifications.Application.Queries.GetUnreadNotificationCount;
using Warehouse.Notifications.Application.Queries.ListNotifications;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<NotificationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var notifications = await _mediator.Send(new ListNotificationsQuery(), cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<GetUnreadNotificationCountResponse>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUnreadNotificationCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var marked = await _mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        if (!marked) return NotFound();

        return NoContent();
    }
}
