namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebApi.Contracts;
using Warehouse.Application.Commands.Products;
using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Exceptions;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyAvailable = false)
        => Ok(await _mediator.Send(new ListProductsQuery(onlyAvailable)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid id format.");

        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Provide at least one of 'name' or 'supplier'.");

        return Ok(await _mediator.Send(new SearchProductsQuery(name, supplier)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _mediator.Send(new CreateProductCommand(
                request.Name, request.SKU, request.Description, request.Price,
                request.QuantityInStock, request.SupplierName, request.ExpiryDate));

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
    {
        try
        {
            var product = await _mediator.Send(new UpdateProductQuantityCommand(id, request.QuantityInStock));
            return product == null ? NotFound() : Ok("Update Done");
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/price")]
    public async Task<IActionResult> UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
    {
        try
        {
            var product = await _mediator.Send(new UpdateProductPriceCommand(id, request.Price));
            return product == null ? NotFound() : Ok("Update done");
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile file)
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

        var product = await _mediator.Send(new UploadProductImageCommand(id, fileName, filePath, ms.ToArray()));
        return product == null ? NotFound() : Ok(new { fileName, filePath });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var product = await _mediator.Send(new ArchiveProductCommand(id));
        return product == null ? NotFound() : Ok("Delete done");
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
    public async Task<IActionResult> AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
    {
        try
        {
            var product = await _mediator.Send(new AssignSupplierToProductCommand(id, supplierId));
            return product == null ? NotFound() : Ok(product);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
