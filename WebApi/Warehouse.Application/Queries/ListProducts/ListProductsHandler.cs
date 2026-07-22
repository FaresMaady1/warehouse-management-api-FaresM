namespace Warehouse.Application.Queries.ListProducts;

using MediatR;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;

    public ListProductsHandler(IProductRepository productRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<List<ProductResponse>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductList(request.OnlyAvailable);
        var cached = await _cache.GetAsync<List<ProductResponse>>(cacheKey, cancellationToken);
        if (cached != null) return cached;

        var products = (await _productRepository.GetAllAsync(cancellationToken)).AsEnumerable();

        if (request.OnlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        var result = products
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.SKU, p.Description, p.Price, p.QuantityInStock,
                p.SupplierName, p.SupplierId, p.ExpiryDate, p.IsArchived, p.CreatedAt, p.LastUpdatedAt))
            .ToList();

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}