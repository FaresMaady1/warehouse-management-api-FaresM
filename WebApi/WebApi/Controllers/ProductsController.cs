namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB;
using WebApi.Contracts;
using WebApi.Models;
using System.Globalization;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    // endpoint 1 get all products (GET/api/products)
    [HttpGet]
    public IActionResult GetAll([FromQuery] bool onlyAvailable = false)
    {
        var products = FakeWarehouseStore.Products.AsEnumerable();
        if (onlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        products = products.OrderByDescending(p => p.CreatedAt);

        return Ok(products.ToList());
    }
    
    // get by id endpoint 2 (GET/api/products/{id})
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] string id)
    {
        if (!Guid.TryParse(id, out _))
            return BadRequest("Invalid id format.");

        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
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

        var results = FakeWarehouseStore.Products.Where(p =>
            (string.IsNullOrWhiteSpace(name) || (p.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)) &&
            (string.IsNullOrWhiteSpace(supplier) || p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        return Ok(results);
    }
    
    // 4. POST product
    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        bool duplicateSku = FakeWarehouseStore.Products
            .Any(p => p.SKU.Equals(request.SKU, StringComparison.OrdinalIgnoreCase));

        if (duplicateSku)
            return BadRequest($"A product with SKU '{request.SKU}' already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            SupplierName = request.SupplierName,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };

        FakeWarehouseStore.Products.Add(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
    
    // 5 update quantity
    [HttpPost("{id}/quantity")]
    public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
    {
        if (request.QuantityInStock < 0)
            return BadRequest("Quantity cannot be negative.");

        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound();

        product.QuantityInStock = request.QuantityInStock;
        product.LastUpdatedAt = DateTime.Now;

        return Ok("Update Done");
    }

    // 6 update price
    [HttpPost("{id}/price")]
    public IActionResult UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
    {
        if (request.Price <= 0)
            return BadRequest("Price must be greater than 0.");

        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound();

        Console.WriteLine($"Price change for {product.Id}: {product.Price} -> {request.Price}");

        product.Price = request.Price;
        product.LastUpdatedAt = DateTime.Now;

        return Ok("Update done");
    }

    // 7 update image 
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile file)
    {
        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
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

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { fileName, filePath });
    }

    // 8 DELETE (soft delete)
    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] string id)
    {
        var product = FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound();

        product.IsArchived = true;
        product.LastUpdatedAt = DateTime.Now;

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
}