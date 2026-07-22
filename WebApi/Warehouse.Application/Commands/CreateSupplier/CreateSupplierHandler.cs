namespace Warehouse.Application.Commands.CreateSupplier;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Suppliers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<CreateSupplierHandler> _logger;

    public CreateSupplierHandler(ISupplierRepository supplierRepository, ICacheService cache, ILogger<CreateSupplierHandler> logger)
    {
        _supplierRepository = supplierRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CreateSupplierResponse> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(request.Name, request.Country, request.ContactEmail, request.PhoneNumber);
        _supplierRepository.Add(supplier);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.SuppliersAll, cancellationToken);

        _logger.LogInformation("Supplier {SupplierId} ({SupplierName}) created", supplier.Id, supplier.Name);

        return new CreateSupplierResponse(
            supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
    }
}