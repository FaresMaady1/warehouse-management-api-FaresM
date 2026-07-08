namespace Warehouse.Domain.Suppliers;

public interface ISupplierRepository
{
    List<Supplier> GetAll();
    Supplier? GetById(string id);
    void Add(Supplier supplier);
}
