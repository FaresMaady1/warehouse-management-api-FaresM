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
    public async Task<ActionResult<List<SupplierViewModel>>> GetAll()
    {
        var suppliers = await _mediator.Send(new ListSuppliersQuery());
        return Ok(_mapper.Map<List<SupplierViewModel>>(suppliers));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierViewModel>> GetById([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id));
        if (supplier == null) return NotFound();

        return Ok(_mapper.Map<SupplierViewModel>(supplier));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierViewModel>> Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = await _mediator.Send(new CreateSupplierCommand(
            request.Name, request.Country, request.ContactEmail, request.PhoneNumber));

        var viewModel = _mapper.Map<SupplierViewModel>(supplier);
        return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SupplierViewModel>> Deactivate([FromRoute] string id)
    {
        var supplier = await _mediator.Send(new DeactivateSupplierCommand(id));
        if (supplier == null) return NotFound();

        return Ok(_mapper.Map<SupplierViewModel>(supplier));
    }
}