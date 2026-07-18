namespace Warehouse.Application.Queries.GetSupplierById;

using MediatR;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cache;

    public GetSupplierByIdHandler(ISupplierRepository supplierRepository, ICacheService cache)
    {
        _supplierRepository = supplierRepository;
        _cache = cache;
    }

    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"suppliers:{request.Id}";
        var cached = await _cache.GetAsync<GetSupplierByIdResponse>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier == null) return null;

        var response = new GetSupplierByIdResponse(supplier.Id, supplier.Name, supplier.Country, supplier.ContactEmail, supplier.PhoneNumber, supplier.IsActive);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        return response;
    }
}