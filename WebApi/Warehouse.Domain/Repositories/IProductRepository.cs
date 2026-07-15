namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.Products;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Product>> SearchAsync(string? name, string? supplier, CancellationToken cancellationToken = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);
    void Add(Product product);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}