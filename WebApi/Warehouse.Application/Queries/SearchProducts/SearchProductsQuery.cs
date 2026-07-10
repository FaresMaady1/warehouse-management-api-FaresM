namespace Warehouse.Application.Queries.SearchProducts;

using MediatR;

public record SearchProductsQuery(string? Name, string? Supplier) : IRequest<List<ProductResponse>>;
