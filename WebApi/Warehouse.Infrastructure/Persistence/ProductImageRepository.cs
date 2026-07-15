namespace Warehouse.Infrastructure.Persistence;

using Warehouse.Domain.ProductImages;
using Warehouse.Domain.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly WarehouseDbContext _context;
    public ProductImageRepository(WarehouseDbContext context) => _context = context;

    public ProductImage? GetByProductId(string productId) => _context.ProductImages.FirstOrDefault(i => i.ProductId == productId);

    public void Add(ProductImage image) => _context.ProductImages.Add(image);

    public void SaveChanges() => _context.SaveChanges();
}