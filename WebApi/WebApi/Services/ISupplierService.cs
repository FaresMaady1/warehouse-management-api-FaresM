namespace WebApi.Services;

using WebApi.Contracts;
using WebApi.Models;

public interface ISupplierService
{
    List<Supplier> GetAll();
    Supplier? GetById(string id);
    Supplier Create(CreateSupplierRequest request);
    void Deactivate(Supplier supplier);
}