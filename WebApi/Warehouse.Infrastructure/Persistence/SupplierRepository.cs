namespace Warehouse.Infrastructure.Persistence;

using Warehouse.Domain.Repositories;
using Warehouse.Domain.Suppliers;

public class SupplierRepository : ISupplierRepository
{
    private readonly WarehouseDbContext _context;
    public SupplierRepository(WarehouseDbContext context) => _context = context;

    public List<Supplier> GetAll() => _context.Suppliers.ToList();

    public Supplier? GetById(string id) => _context.Suppliers.FirstOrDefault(s => s.Id == id);

    public void Add(Supplier supplier) => _context.Suppliers.Add(supplier);

    public void SaveChanges() => _context.SaveChanges();
}