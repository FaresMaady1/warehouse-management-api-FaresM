namespace Warehouse.Application.Queries.GetProductById;

public record GetProductByIdResponse(
    string Id, string Name, string SKU, string Description, decimal Price,
    int QuantityInStock, string SupplierName, string? SupplierId, DateTime ExpiryDate,
    bool IsArchived, DateTime CreatedAt, DateTime LastUpdatedAt);
