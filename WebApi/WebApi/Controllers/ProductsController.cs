namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Services;
using System.Globalization;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // endpoint 1 get all products (GET/api/products)
    [HttpGet]
    public IActionResult GetAll([FromQuery] bool onlyAvailable = false)
    {
        return Ok(_productService.GetAll(onlyAvailable));
    }

    // get by id endpoint 2 (GET/api/products/{id})
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid id format.");

        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // endpoint 3 search
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name, [FromQuery] string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return BadRequest("Provide at least one of 'name' or 'supplier'.");

        var results = _productService.Search(name, supplier);

        return Ok(results);
    }

    // 4. POST product
    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        if (_productService.SkuExists(request.SKU))
            return BadRequest($"A product with SKU '{request.SKU}' already exists.");

        var product = _productService.Create(request);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // 5 update quantity
    [HttpPost("{id}/quantity")]
    public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
    {
        if (request.QuantityInStock < 0)
            return BadRequest("Quantity cannot be negative.");

        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();

        _productService.UpdateQuantity(product, request);

        return Ok("Update Done");
    }

    // 6 update price
    [HttpPost("{id}/price")]
    public IActionResult UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
    {
        if (request.Price <= 0)
            return BadRequest("Price must be greater than 0.");

        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();

        _productService.UpdatePrice(product, request);

        return Ok("Update done");
    }

    // 7 update image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile file)
    {
        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".jpg" && ext != ".png")
            return BadRequest("Only .jpg, .png files are allowed.");

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest("Max file size is 2 MB.");

        var uploadsFolder = Path.Combine("wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{id}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await _productService.SaveImageAsync(product, file, fileName, filePath);

        return Ok(new { fileName, filePath });
    }

    // 8 DELETE (soft delete)
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] string id)
    {
        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();

        _productService.Delete(product);

        return Ok("Delete done");
    }

    // 9 get time
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

    // assign supplier
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public IActionResult AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
    {
        var product = _productService.GetById(id);
        if (product == null)
            return NotFound("Product not found.");

        var supplier = _productService.GetSupplierById(supplierId);
        if (supplier == null)
            return NotFound("Supplier not found.");

        if (product.IsArchived)
            return BadRequest("Cannot assign a supplier to an archived product.");

        _productService.AssignSupplier(product, supplier);

        return Ok(product);
    }
}