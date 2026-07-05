namespace WebApi.Services;

using WebApi.Contracts;
using WebApi.DB;
using WebApi.Models;

public class SupplierService : ISupplierService
{
    public List<Supplier> GetAll()
    {
        return FakeSupplierStore.Suppliers;
    }

    public Supplier? GetById(string id)
    {
        return FakeSupplierStore.Suppliers.FirstOrDefault(s => s.Id == id);
    }

    public Supplier Create(CreateSupplierRequest request)
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

        return supplier;
    }

    public void Deactivate(Supplier supplier)
    {
        supplier.IsActive = false;
    }
}