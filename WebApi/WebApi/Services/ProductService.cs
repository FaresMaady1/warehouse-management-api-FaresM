namespace WebApi.Services;

using WebApi.Contracts;
using WebApi.DB;
using WebApi.Models;

public class ProductService : IProductService
{
    public List<Product> GetAll(bool onlyAvailable)
    {
        var products = FakeWarehouseStore.Products.AsEnumerable();

        if (onlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        return products.OrderByDescending(p => p.CreatedAt).ToList();
    }

    public Product? GetById(string id)
    {
        return FakeWarehouseStore.Products.FirstOrDefault(p => p.Id == id);
    }

    public List<Product> Search(string? name, string? supplier)
    {
        return FakeWarehouseStore.Products.Where(p =>
            (string.IsNullOrWhiteSpace(name) || (p.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false)) &&
            (string.IsNullOrWhiteSpace(supplier) || p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public bool SkuExists(string sku)
    {
        return FakeWarehouseStore.Products.Any(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));
    }

    public Product Create(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            SupplierName = request.SupplierName,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };

        FakeWarehouseStore.Products.Add(product);

        return product;
    }

    public void UpdateQuantity(Product product, UpdateProductQuantityRequest request)
    {
        product.QuantityInStock = request.QuantityInStock;
        product.LastUpdatedAt = DateTime.Now;
    }

    public void UpdatePrice(Product product, UpdateProductPriceRequest request)
    {
        Console.WriteLine($"Price change for {product.Id}: {product.Price} -> {request.Price}");

        product.Price = request.Price;
        product.LastUpdatedAt = DateTime.Now;
    }

    public async Task SaveImageAsync(Product product, IFormFile file, string fileName, string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    public void Delete(Product product)
    {
        product.IsArchived = true;
        product.LastUpdatedAt = DateTime.Now;
    }

    public Supplier? GetSupplierById(string supplierId)
    {
        return FakeSupplierStore.Suppliers.FirstOrDefault(s => s.Id == supplierId);
    }

    public void AssignSupplier(Product product, Supplier supplier)
    {
        product.SupplierId = supplier.Id;
        product.SupplierName = supplier.Name;
        product.LastUpdatedAt = DateTime.Now;
    }
}