namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB;

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
}