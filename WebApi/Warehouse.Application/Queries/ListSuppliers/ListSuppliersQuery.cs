namespace Warehouse.Application.Queries.ListSuppliers;

using MediatR;

public record ListSuppliersQuery() : IRequest<List<SupplierResponse>>;
