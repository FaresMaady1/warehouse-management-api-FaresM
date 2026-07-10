namespace Warehouse.Application.Commands.UpdateProductQuantity;

public record UpdateProductQuantityResponse(
    string Id, string Name, string SKU, string Description, decimal Price,
    int QuantityInStock, string SupplierName, string? SupplierId, DateTime ExpiryDate,
    bool IsArchived, DateTime CreatedAt, DateTime LastUpdatedAt);
