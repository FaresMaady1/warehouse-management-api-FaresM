namespace Warehouse.Application.Commands.CreateSupplier;

using MediatR;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Suppliers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    public CreateSupplierHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public async Task<CreateSupplierResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(request.Name, request.Country, request.ContactEmail, request.PhoneNumber);
        _supplierRepository.Add(supplier);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return new CreateSupplierResponse(
            supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
    }
}