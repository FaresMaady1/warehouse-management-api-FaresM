namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB;
using WebApi.Contracts;
using WebApi.Models;

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
    
    // 4. POST /api/products
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
}