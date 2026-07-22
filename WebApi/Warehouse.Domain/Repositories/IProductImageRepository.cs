namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.ProductImages;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default);
    void Add(ProductImage image);
    void Remove(ProductImage image);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}