namespace Warehouse.Application.Queries.Suppliers;

using MediatR;
using Warehouse.Domain.Suppliers;

public record ListSuppliersQuery() : IRequest<List<Supplier>>;
