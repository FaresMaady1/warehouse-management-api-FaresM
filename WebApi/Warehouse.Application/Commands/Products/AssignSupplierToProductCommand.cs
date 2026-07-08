namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record AssignSupplierToProductCommand(string ProductId, string SupplierId) : IRequest<Product?>;
