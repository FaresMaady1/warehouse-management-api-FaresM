namespace Warehouse.Application.Queries.GetSupplierById;

using MediatR;
using Warehouse.Domain.Repositories;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    public GetSupplierByIdHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier == null) return null;

        return new GetSupplierByIdResponse(supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
    }
}