namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using Warehouse.Application.Commands.CreateSupplier;
using Warehouse.Application.Commands.DeactivateSupplier;
using Warehouse.Application.Queries.ListSuppliers;
using Warehouse.Application.Queries.GetSupplierById;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SuppliersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _mediator.Send(new ListSuppliersQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
        return supplier == null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = await _mediator.Send(new CreateSupplierCommand(
            request.Name, request.Country, request.ContactEmail, request.PhoneNumber));

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new DeactivateSupplierCommand(id));
        return supplier == null ? NotFound() : Ok("Delete Done");
    }
}
