namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;
    public ProductRepository(WarehouseDbContext context) => _context = context;

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Products.ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<List<Product>> SearchAsync(string? name, string? supplier, CancellationToken cancellationToken = default)
    {
        return _context.Products.Where(p =>
            (string.IsNullOrWhiteSpace(name) || EF.Functions.ILike(p.Name, $"%{name}%")) &&
            (string.IsNullOrWhiteSpace(supplier) || EF.Functions.ILike(p.SupplierName, $"%{supplier}%"))
        ).ToListAsync(cancellationToken);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default) =>
        _context.Products.AnyAsync(p => p.SKU == sku, cancellationToken);

    public void Add(Product product) => _context.Products.Add(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}