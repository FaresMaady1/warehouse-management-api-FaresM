namespace Warehouse.Application.Commands.DeactivateSupplier;

using MediatR;
using Warehouse.Domain.Repositories;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierCommand, DeactivateSupplierResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    public DeactivateSupplierHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<DeactivateSupplierResponse?> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = _supplierRepository.GetById(request.SupplierId);
        if (supplier == null) return Task.FromResult<DeactivateSupplierResponse?>(null);

        supplier.Deactivate();
        _supplierRepository.SaveChanges();
        return Task.FromResult<DeactivateSupplierResponse?>(new DeactivateSupplierResponse(
            supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive));
    }
}
