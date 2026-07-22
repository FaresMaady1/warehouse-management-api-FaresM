namespace WebApi.Controllers;

using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using WebApi.Contracts;
using WebApi.ViewModels;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Application.Commands.UpdateProductQuantity;
using Warehouse.Application.Commands.UpdateProductPrice;
using Warehouse.Application.Commands.ArchiveProduct;
using Warehouse.Application.Commands.AssignSupplierToProduct;
using Warehouse.Application.Commands.UploadProductImage;
using Warehouse.Application.Queries.GetProductById;
using Warehouse.Application.Queries.ListProducts;
using Warehouse.Application.Queries.SearchProducts;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public ProductsController(IMediator mediator, IMapper mapper, IStringLocalizer localizer)
    {
        _mediator = mediator;
        _mapper = mapper;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductViewModel>>> GetAll([FromQuery] bool onlyAvailable = false, CancellationToken cancellationToken = default)
    {
        var products = await _mediator.Send(new ListProductsQuery(onlyAvailable), cancellationToken);
        return Ok(_mapper.Map<List<ProductViewModel>>(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductViewModel>> GetById([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest(_localizer["InvalidIdFormat"].Value);

        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        if (product == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductViewModel>>> Search([FromQuery] string? name, [FromQuery] string? supplier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Provide at least one of 'name' or 'supplier'.");

        var products = await _mediator.Send(new SearchProductsQuery(name, supplier), cancellationToken);
        return Ok(_mapper.Map<List<ProductViewModel>>(products));
    }

    [HttpPost]
    public async Task<ActionResult<ProductViewModel>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new CreateProductCommand(
            request.Name, request.SKU, request.Description, request.Price,
            request.QuantityInStock, request.SupplierName, request.ExpiryDate), cancellationToken);

        var viewModel = _mapper.Map<ProductViewModel>(product);
        return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
    }

    [HttpPost("{id}/quantity")]
    public async Task<ActionResult<ProductViewModel>> UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new UpdateProductQuantityCommand(id, request.QuantityInStock), cancellationToken);
        if (product == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpPost("{id}/price")]
    public async Task<ActionResult<ProductViewModel>> UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new UpdateProductPriceCommand(id, request.Price), cancellationToken);
        if (product == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpPost("{id}/image")]
    public async Task<ActionResult<UploadProductImageResponse>> UploadImage([FromRoute] string id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".jpg" && ext != ".png")
            return BadRequest("Only .jpg, .png files are allowed.");

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest("Max file size is 2 MB.");

        var fileName = $"{id}{ext}";
        var filePath = Path.Combine("wwwroot", "uploads", fileName);

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);

        var result = await _mediator.Send(new UploadProductImageCommand(id, fileName, filePath, ms.ToArray()), cancellationToken);
        if (result == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProductViewModel>> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new ArchiveProductCommand(id), cancellationToken);
        if (product == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpGet("server-time")]
    public IActionResult GetServerTime([FromHeader(Name = "Accept-Language")] string? acceptLanguage)
    {
        string culture = acceptLanguage switch
        {
            "fr" => "fr-FR",
            "ar" => "ar-LB",
            _ => "en-US"
        };

        var formatted = DateTime.Now.ToString("F", new CultureInfo(culture));
        return Ok(new { culture, serverTime = formatted });
    }

    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<ActionResult<ProductViewModel>> AssignSupplier([FromRoute] string id, [FromRoute] string supplierId, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new AssignSupplierToProductCommand(id, supplierId), cancellationToken);
        if (product == null) return NotFound(_localizer["ProductNotFound"].Value);

        return Ok(_mapper.Map<ProductViewModel>(product));
    }
}