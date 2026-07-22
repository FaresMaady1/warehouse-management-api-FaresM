namespace Warehouse.Application.Commands.DeactivateSupplier;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierCommand, DeactivateSupplierResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<DeactivateSupplierHandler> _logger;

    public DeactivateSupplierHandler(ISupplierRepository supplierRepository, ICacheService cache, ILogger<DeactivateSupplierHandler> logger)
    {
        _supplierRepository = supplierRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<DeactivateSupplierResponse?> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) return null;

        supplier.Deactivate();
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.SuppliersAll, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Supplier(supplier.Id), cancellationToken);

        _logger.LogInformation("Supplier {SupplierId} deactivated", supplier.Id);

        return new DeactivateSupplierResponse(
            supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
    }
}