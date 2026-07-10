namespace Warehouse.Application.Queries.GetSupplierById;

using MediatR;
using Warehouse.Domain.Repositories;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    public GetSupplierByIdHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = _supplierRepository.GetById(request.Id);
        if (supplier == null) return Task.FromResult<GetSupplierByIdResponse?>(null);

        return Task.FromResult<GetSupplierByIdResponse?>(
            new GetSupplierByIdResponse(supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive));
    }
}
