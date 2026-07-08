namespace Warehouse.Application.Queries.Products;

using MediatR;
using Warehouse.Domain.Products;

public record ListProductsQuery(bool OnlyAvailable) : IRequest<List<Product>>;
