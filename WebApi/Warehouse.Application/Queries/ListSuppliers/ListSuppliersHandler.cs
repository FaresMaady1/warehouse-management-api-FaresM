namespace Warehouse.Application.Queries.ListSuppliers;

using MediatR;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<SupplierResponse>>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cache;

    public ListSuppliersHandler(ISupplierRepository supplierRepository, ICacheService cache)
    {
        _supplierRepository = supplierRepository;
        _cache = cache;
    }

    public async Task<List<SupplierResponse>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync<List<SupplierResponse>>("suppliers:all", cancellationToken);
        if (cached != null) return cached;

        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

        var result = suppliers
            .Select(s => new SupplierResponse(s.Id, s.Name, s.Country, s.ContactEmail, s.PhoneNumber, s.IsActive))
            .ToList();

        await _cache.SetAsync("suppliers:all", result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}