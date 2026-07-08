namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record UpdateProductQuantityCommand(string ProductId, int QuantityInStock) : IRequest<Product?>;
