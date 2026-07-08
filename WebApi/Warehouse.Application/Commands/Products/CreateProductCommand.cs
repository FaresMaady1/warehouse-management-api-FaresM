namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record CreateProductCommand(
    string Name, string SKU, string Description, decimal Price,
    int QuantityInStock, string SupplierName, DateTime ExpiryDate) : IRequest<Product>;
