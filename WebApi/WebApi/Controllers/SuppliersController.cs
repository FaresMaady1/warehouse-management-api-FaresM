namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Services;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // get all suppliers
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_supplierService.GetAll());
    }

    // get a supplier by id
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] string id)
    {
        var supplier = _supplierService.GetById(id);
        if (supplier == null)
            return NotFound();

        return Ok(supplier);
    }

    // add a supplier
    [HttpPost]
    public IActionResult Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = _supplierService.Create(request);

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    // Soft delete
    [HttpDelete("{id}")]
    public IActionResult Deactivate([FromRoute] string id)
    {
        var supplier = _supplierService.GetById(id);
        if (supplier == null)
            return NotFound();

        _supplierService.Deactivate(supplier);

        return Ok("Delete Done");
    }
}