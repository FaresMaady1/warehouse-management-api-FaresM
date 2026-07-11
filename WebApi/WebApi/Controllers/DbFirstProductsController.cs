namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbFirst;
[ApiController]
[Route("api/dbfirst/products")]
public class DbFirstProductsController : ControllerBase
{
    private readonly WarehouseDbFirstContext _context;
    public DbFirstProductsController(WarehouseDbFirstContext context) => _context = context;
// using lambda expression for all linq
    [HttpGet("by-supplier")]
    public async Task<IActionResult> GetBySupplier([FromQuery] string supplierName, [FromQuery] string sortOrder = "asc")
    {
        var query = _context.Products.Include(p => p.Supplier).Where(p => p.Supplier != null && p.Supplier.Name == supplierName);

        query = sortOrder.ToLower() == "desc"
            ? query.OrderByDescending(p => p.CreatedAt)
            : query.OrderBy(p => p.CreatedAt);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("group-by-expiry-year")]
    public async Task<IActionResult> GroupByExpiryYear()
    {
        var result = await _context.Products
            .GroupBy(p => p.ExpiryDate.Year)
            .Select(g => new { Year = g.Key, Count = g.Count(), Products = g.ToList() })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("group-by-expiry-year-and-country")]
    public async Task<IActionResult> GroupByExpiryYearAndCountry()
    {
        var result = await _context.Products
            .Include(p => p.Supplier)
            .GroupBy(p => new { p.ExpiryDate.Year, Country = p.Supplier != null ? p.Supplier.Country : null })
            .Select(g => new { g.Key.Year, g.Key.Country, Count = g.Count() })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        return Ok(await _context.Products.CountAsync());
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _context.Products
            .OrderBy(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(products);
    }
}