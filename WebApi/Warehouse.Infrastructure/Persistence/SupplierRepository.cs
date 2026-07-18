namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Suppliers;

public class SupplierRepository : ISupplierRepository
{
    private readonly WarehouseDbContext _context;
    public SupplierRepository(WarehouseDbContext context) => _context = context;

    public Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Suppliers.ToListAsync(cancellationToken);

    public Task<Supplier?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public void Add(Supplier supplier) => _context.Suppliers.Add(supplier);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}