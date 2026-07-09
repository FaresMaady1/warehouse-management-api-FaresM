namespace Warehouse.Application.Queries.ListSuppliers;

using MediatR;
using Warehouse.Domain.Repositories;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<SupplierResponse>>
{
    private readonly ISupplierRepository _supplierRepository;
    public ListSuppliersHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<List<SupplierResponse>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var response = _supplierRepository.GetAll()
            .Select(s => new SupplierResponse(s.Id, s.Name, s.Country, s.ContactEmail, s.PhoneNumber, s.IsActive))
            .ToList();

        return Task.FromResult(response);
    }
}
