namespace Warehouse.Application.Commands.CreateProduct;

public record CreateProductResponse(
    string Id, string Name, string SKU, string Description, decimal Price,
    int QuantityInStock, string SupplierName, string? SupplierId, DateTime ExpiryDate,
    bool IsArchived, DateTime CreatedAt, DateTime LastUpdatedAt);
