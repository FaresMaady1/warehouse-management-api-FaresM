namespace Warehouse.Application.Queries.Suppliers;

using MediatR;
using Warehouse.Domain.Suppliers;

public record GetSupplierByIdQuery(string Id) : IRequest<Supplier?>;
