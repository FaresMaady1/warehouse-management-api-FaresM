namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record ArchiveProductCommand(string ProductId) : IRequest<Product?>;
