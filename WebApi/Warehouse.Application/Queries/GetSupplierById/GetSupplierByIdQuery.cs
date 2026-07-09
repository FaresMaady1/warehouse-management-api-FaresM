namespace Warehouse.Application.Queries.GetSupplierById;

using MediatR;

public record GetSupplierByIdQuery(string Id) : IRequest<GetSupplierByIdResponse?>;
