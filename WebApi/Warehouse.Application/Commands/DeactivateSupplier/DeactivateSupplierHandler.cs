namespace Warehouse.Application.Commands.DeactivateSupplier;

using MediatR;
using Warehouse.Domain.Repositories;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierCommand, DeactivateSupplierResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    public DeactivateSupplierHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public async Task<DeactivateSupplierResponse?> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) return null;

        supplier.Deactivate();
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return new DeactivateSupplierResponse(
            supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
    }
}