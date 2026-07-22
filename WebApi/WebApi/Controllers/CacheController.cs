namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Queries.GetCacheStatistics;

[ApiController]
[Route("api/cache")]
public class CacheController : ControllerBase
{
    private readonly IMediator _mediator;
    public CacheController(IMediator mediator) => _mediator = mediator;

    [HttpGet("statistics")]
    public async Task<ActionResult<GetCacheStatisticsResponse>> GetStatistics(CancellationToken cancellationToken)
    {
        var stats = await _mediator.Send(new GetCacheStatisticsQuery(), cancellationToken);
        return Ok(stats);
    }
}