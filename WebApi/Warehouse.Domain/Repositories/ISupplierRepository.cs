namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.Suppliers;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    void Add(Supplier supplier);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}