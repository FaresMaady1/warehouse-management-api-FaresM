namespace Warehouse.Application.Queries.Products;

using MediatR;
using Warehouse.Domain.Products;

public record SearchProductsQuery(string? Name, string? Supplier) : IRequest<List<Product>>;
