namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.ProductImages;
using Warehouse.Domain.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly WarehouseDbContext _context;
    public ProductImageRepository(WarehouseDbContext context) => _context = context;

    public Task<ProductImage?> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default) =>
        _context.ProductImages.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);

    public void Add(ProductImage image) => _context.ProductImages.Add(image);

    public void Remove(ProductImage image) => _context.ProductImages.Remove(image);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}