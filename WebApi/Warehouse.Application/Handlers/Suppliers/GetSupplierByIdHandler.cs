namespace Warehouse.Application.Handlers.Suppliers;

using MediatR;
using Warehouse.Application.Queries.Suppliers;
using Warehouse.Domain.Suppliers;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, Supplier?>
{
    private readonly ISupplierRepository _supplierRepository;
    public GetSupplierByIdHandler(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    public Task<Supplier?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_supplierRepository.GetById(request.Id));
}
