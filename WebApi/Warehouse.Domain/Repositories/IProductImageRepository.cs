namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.ProductImages;

public interface IProductImageRepository
{
    ProductImage? GetByProductId(string productId);
    void Add(ProductImage image);
    void SaveChanges();
}