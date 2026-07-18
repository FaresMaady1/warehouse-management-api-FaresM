namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Queries.GetInventoryDashboard;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public InventoryController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<ActionResult<GetInventoryDashboardResponse>> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _mediator.Send(new GetInventoryDashboardQuery(), cancellationToken);
        return Ok(dashboard);
    }
}