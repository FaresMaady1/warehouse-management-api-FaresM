namespace Warehouse.Application.Queries.Products;

using MediatR;
using Warehouse.Domain.Products;

public record GetProductByIdQuery(string Id) : IRequest<Product?>;
