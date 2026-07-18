namespace WebApi.Controllers;

using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
    private readonly IStringLocalizer _localizer;

    public SuppliersController(IMediator mediator, IMapper mapper, IStringLocalizer localizer)
    {
        _mediator = mediator;
        _mapper = mapper;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<List<SupplierViewModel>>> GetAll(CancellationToken cancellationToken)
    {
        var suppliers = await _mediator.Send(new ListSuppliersQuery(), cancellationToken);
        return Ok(_mapper.Map<List<SupplierViewModel>>(suppliers));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierViewModel>> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(new GetSupplierByIdQuery(id), cancellationToken);
        if (supplier == null) return NotFound(_localizer["SupplierNotFound"].Value);

        return Ok(_mapper.Map<SupplierViewModel>(supplier));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierViewModel>> Create([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(new CreateSupplierCommand(
            request.Name, request.Country, request.ContactEmail, request.PhoneNumber), cancellationToken);

        var viewModel = _mapper.Map<SupplierViewModel>(supplier);
        return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<SupplierViewModel>> Deactivate([FromRoute] string id, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(new DeactivateSupplierCommand(id), cancellationToken);
        if (supplier == null) return NotFound(_localizer["SupplierNotFound"].Value);

        return Ok(_mapper.Map<SupplierViewModel>(supplier));
    }
}