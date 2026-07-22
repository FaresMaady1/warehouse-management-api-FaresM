namespace WebApi.Controllers;

using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebApi.Contracts;
using WebApi.ViewModels;
using Warehouse.Application.Commands.CreateSupplier;
using Warehouse.Application.Commands.DeactivateSupplier;
using Warehouse.Application.Commands.UploadSupplierDocument;
using Warehouse.Application.Commands.DeleteSupplierDocument;
using Warehouse.Application.Queries.DownloadSupplierDocument;
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

    [HttpGet("documents/{documentId}")]
    public async Task<IActionResult> DownloadDocument([FromRoute] string documentId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadSupplierDocumentQuery(documentId), cancellationToken);
        if (result == null) return NotFound("Document not found.");

        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SupplierViewModel>> Create([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(new CreateSupplierCommand(
            request.Name, request.Country, request.ContactEmail, request.PhoneNumber), cancellationToken);

        var viewModel = _mapper.Map<SupplierViewModel>(supplier);
        return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
    }

    [HttpPost("{id}/documents")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UploadSupplierDocumentResponse>> UploadDocument([FromRoute] string id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".pdf" && ext != ".docx")
            return BadRequest("Only .pdf, .docx files are allowed.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Max file size is 5 MB.");

        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadSupplierDocumentCommand(id, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
        if (result == null) return NotFound(_localizer["SupplierNotFound"].Value);

        return Ok(result);
    }

    [HttpDelete("documents/{documentId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteDocument([FromRoute] string documentId, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSupplierDocumentCommand(documentId), cancellationToken);
        if (!deleted) return NotFound("Document not found.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<SupplierViewModel>> Deactivate([FromRoute] string id, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(new DeactivateSupplierCommand(id), cancellationToken);
        if (supplier == null) return NotFound(_localizer["SupplierNotFound"].Value);

        return Ok(_mapper.Map<SupplierViewModel>(supplier));
    }
}