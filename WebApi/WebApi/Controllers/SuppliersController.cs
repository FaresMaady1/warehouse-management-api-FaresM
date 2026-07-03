namespace WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.DB;
using WebApi.Models;
[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    // get all suppliers
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(FakeSupplierStore.Suppliers);
    }

    // get a supplier by id
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] string id)
    {
        var supplier = FakeSupplierStore.Suppliers.FirstOrDefault(s => s.Id == id);
        if (supplier == null)
            return NotFound();

        return Ok(supplier);
    }

    // add a supplier
    [HttpPost]
    public IActionResult Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        FakeSupplierStore.Suppliers.Add(supplier);

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    // Soft delete
    [HttpDelete("{id}")]
    public IActionResult Deactivate([FromRoute] string id)
    {
        var supplier = FakeSupplierStore.Suppliers.FirstOrDefault(s => s.Id == id);
        if (supplier == null)
            return NotFound();

        supplier.IsActive = false;

        return Ok("Delete Done");
    }
}