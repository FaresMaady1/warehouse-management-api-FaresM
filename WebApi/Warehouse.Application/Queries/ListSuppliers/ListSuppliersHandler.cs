namespace Warehouse.Application.Queries.ListSuppliers;

using MediatR;
using Warehouse.Domain.Repositories;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<SupplierResponse>>
{
    private readonly ISupplierRepository _supplierRepository;
    public ListSuppliersHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public async Task<List<SupplierResponse>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

        return suppliers
            .Select(s => new SupplierResponse(s.Id, s.Name, s.Country, s.ContactEmail, s.PhoneNumber, s.IsActive))
            .ToList();
    }
}