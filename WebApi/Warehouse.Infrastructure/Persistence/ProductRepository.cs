namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;
    public ProductRepository(WarehouseDbContext context) => _context = context;

    public List<Product> GetAll() => _context.Products.ToList();

    public Product? GetById(string id) => _context.Products.FirstOrDefault(p => p.Id == id);

    public List<Product> Search(string? name, string? supplier)
    {
        return _context.Products.Where(p =>
            (string.IsNullOrWhiteSpace(name) || EF.Functions.ILike(p.Name, $"%{name}%")) &&
            (string.IsNullOrWhiteSpace(supplier) || EF.Functions.ILike(p.SupplierName, $"%{supplier}%"))
        ).ToList();
    }

    public bool SkuExists(string sku) => _context.Products.Any(p => p.SKU == sku);

    public void Add(Product product) => _context.Products.Add(product);

    public void SaveChanges() => _context.SaveChanges();
}