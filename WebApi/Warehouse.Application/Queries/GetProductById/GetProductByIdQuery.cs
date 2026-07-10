namespace Warehouse.Application.Queries.GetProductById;

using MediatR;

public record GetProductByIdQuery(string Id) : IRequest<GetProductByIdResponse?>;
