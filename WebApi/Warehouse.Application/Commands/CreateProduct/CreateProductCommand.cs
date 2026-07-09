namespace Warehouse.Application.Commands.CreateProduct;

using MediatR;

public record CreateProductCommand(
    string Name, string SKU, string Description, decimal Price,
    int QuantityInStock, string SupplierName, DateTime ExpiryDate) : IRequest<CreateProductResponse>;
