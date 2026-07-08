namespace Warehouse.Application.Handlers.Suppliers;

using MediatR;
using Warehouse.Application.Commands.Suppliers;
using Warehouse.Domain.Suppliers;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierCommand, Supplier?>
{
    private readonly ISupplierRepository _supplierRepository;
    public DeactivateSupplierHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<Supplier?> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = _supplierRepository.GetById(request.SupplierId);
        if (supplier == null) return Task.FromResult<Supplier?>(null);

        supplier.Deactivate();
        return Task.FromResult<Supplier?>(supplier);
    }
}
