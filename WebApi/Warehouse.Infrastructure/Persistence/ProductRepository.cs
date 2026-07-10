namespace Warehouse.Infrastructure.Persistence;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;


public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;
    public ProductRepository(WarehouseDbContext context) => _context = context;

    public List<Product> GetAll() => _context.Products;

    public Product? GetById(string id) => _context.Products.FirstOrDefault(p => p.Id == id);

    public List<Product> Search(string? name, string? supplier)
    {
        return _context.Products.Where(p =>
            (string.IsNullOrWhiteSpace(name) || (p.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)) &&
            (string.IsNullOrWhiteSpace(supplier) || p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public bool SkuExists(string sku) => _context.Products.Any(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

    public void Add(Product product) => _context.Products.Add(product);
}
