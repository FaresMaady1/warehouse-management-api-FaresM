namespace Warehouse.Application.Handlers.Suppliers;

using MediatR;
using Warehouse.Application.Commands.Suppliers;
using Warehouse.Domain.Suppliers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Supplier>
{
    private readonly ISupplierRepository _supplierRepository;
    public CreateSupplierHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<Supplier> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(request.Name, request.Country, request.ContactEmail, request.PhoneNumber);
        _supplierRepository.Add(supplier);
        return Task.FromResult(supplier);
    }
}
