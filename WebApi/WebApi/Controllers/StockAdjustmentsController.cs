namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using Warehouse.Application.Commands.CreateStockAdjustment;

[ApiController]
[Route("api/stock-adjustments")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public StockAdjustmentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<CreateStockAdjustmentResponse>> Create([FromBody] CreateStockAdjustmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateStockAdjustmentCommand(request.ProductId, request.QuantityChanged, request.Reason ?? string.Empty),
            cancellationToken);

        return Ok(result);
    }
}