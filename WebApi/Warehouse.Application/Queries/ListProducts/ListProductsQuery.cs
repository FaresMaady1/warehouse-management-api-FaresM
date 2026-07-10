namespace Warehouse.Application.Queries.ListProducts;

using MediatR;

public record ListProductsQuery(bool OnlyAvailable) : IRequest<List<ProductResponse>>;
