namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record UpdateProductPriceCommand(string ProductId, decimal Price) : IRequest<Product?>;
