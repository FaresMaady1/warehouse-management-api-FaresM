namespace WebApi.Services;

using WebApi.Contracts;
using WebApi.Models;

public interface IProductService
{
    List<Product> GetAll(bool onlyAvailable);
    Product? GetById(string id);
    List<Product> Search(string? name, string? supplier);
    bool SkuExists(string sku);
    Product Create(CreateProductRequest request);
    void UpdateQuantity(Product product, UpdateProductQuantityRequest request);
    void UpdatePrice(Product product, UpdateProductPriceRequest request);
    Task SaveImageAsync(Product product, IFormFile file, string fileName, string filePath);
    void Delete(Product product);
    Supplier? GetSupplierById(string supplierId);
    void AssignSupplier(Product product, Supplier supplier);
}