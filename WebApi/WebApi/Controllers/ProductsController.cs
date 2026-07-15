namespace WebApi.Controllers;

using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
using Warehouse.Domain.Exceptions;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductViewModel>>> GetAll([FromQuery] bool onlyAvailable = false)
    {
        var products = await _mediator.Send(new ListProductsQuery(onlyAvailable));
        return Ok(_mapper.Map<List<ProductViewModel>>(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductViewModel>> GetById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid id format.");

        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null) return NotFound();

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductViewModel>>> Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Provide at least one of 'name' or 'supplier'.");

        var products = await _mediator.Send(new SearchProductsQuery(name, supplier));
        return Ok(_mapper.Map<List<ProductViewModel>>(products));
    }

    [HttpPost]
    public async Task<ActionResult<ProductViewModel>> Create([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _mediator.Send(new CreateProductCommand(
                request.Name, request.SKU, request.Description, request.Price,
                request.QuantityInStock, request.SupplierName, request.ExpiryDate));

            var viewModel = _mapper.Map<ProductViewModel>(product);
            return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/quantity")]
    public async Task<ActionResult<ProductViewModel>> UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
    {
        try
        {
            var product = await _mediator.Send(new UpdateProductQuantityCommand(id, request.QuantityInStock));
            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductViewModel>(product));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/price")]
    public async Task<ActionResult<ProductViewModel>> UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
    {
        try
        {
            var product = await _mediator.Send(new UpdateProductPriceCommand(id, request.Price));
            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductViewModel>(product));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/image")]
    public async Task<ActionResult<UploadProductImageResponse>> UploadImage([FromRoute] string id, IFormFile file)
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
        await file.CopyToAsync(ms);

        var result = await _mediator.Send(new UploadProductImageCommand(id, fileName, filePath, ms.ToArray()));
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProductViewModel>> Delete([FromRoute] string id)
    {
        var product = await _mediator.Send(new ArchiveProductCommand(id));
        if (product == null) return NotFound();

        return Ok(_mapper.Map<ProductViewModel>(product));
    }

    [HttpGet("server-time")]
    public IActionResult GetServerTime([FromHeader(Name = "Accept-Language")] string? acceptLanguage)
    {
        string culture = acceptLanguage switch
        {
            "fr-FR" => "fr-FR",
            "ar-LB" => "ar-LB",
            _ => "en-US"
        };

        var formatted = DateTime.Now.ToString("F", new CultureInfo(culture));
        return Ok(new { culture, serverTime = formatted });
    }

    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public async Task<ActionResult<ProductViewModel>> AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
    {
        try
        {
            var product = await _mediator.Send(new AssignSupplierToProductCommand(id, supplierId));
            if (product == null) return NotFound();

            return Ok(_mapper.Map<ProductViewModel>(product));
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}