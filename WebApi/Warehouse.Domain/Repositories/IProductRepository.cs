namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.Products;

public interface IProductRepository
{
    List<Product> GetAll();
    Product? GetById(string id);
    List<Product> Search(string? name, string? supplier);
    bool SkuExists(string sku);
    void Add(Product product);
    void SaveChanges();
}
