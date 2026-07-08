namespace Warehouse.Application.Handlers.Suppliers;

using MediatR;
using Warehouse.Application.Queries.Suppliers;
using Warehouse.Domain.Suppliers;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<Supplier>>
{
    private readonly ISupplierRepository _supplierRepository;
    public ListSuppliersHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<List<Supplier>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_supplierRepository.GetAll());
}
