namespace WebApi.Controllers;

using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.ViewModels;
using Warehouse.Application.Commands.CreateSupplier;
using Warehouse.Application.Commands.DeactivateSupplier;
using Warehouse.Application.Queries.ListSuppliers;
using Warehouse.Application.Queries.GetSupplierById;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public SuppliersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _mediator.Send(new ListSuppliersQuery());
        return Ok(_mapper.Map<List<SupplierViewModel>>(suppliers));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
        return supplier == null ? NotFound() : Ok(_mapper.Map<SupplierViewModel>(supplier));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = await _mediator.Send(new CreateSupplierCommand(
            request.Name, request.Country, request.ContactEmail, request.PhoneNumber));

        var viewModel = _mapper.Map<SupplierViewModel>(supplier);
        return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new DeactivateSupplierCommand(id));
        return supplier == null ? NotFound() : Ok(_mapper.Map<SupplierViewModel>(supplier));
    }
}