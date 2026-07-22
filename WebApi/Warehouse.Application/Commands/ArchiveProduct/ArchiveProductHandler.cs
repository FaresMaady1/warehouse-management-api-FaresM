namespace Warehouse.Application.Commands.ArchiveProduct;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, ArchiveProductResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<ArchiveProductHandler> _logger;

    public ArchiveProductHandler(IProductRepository productRepository, ICacheService cache, ILogger<ArchiveProductHandler> logger)
    {
        _productRepository = productRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ArchiveProductResponse?> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        product.Archive();
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.Product(product.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(true), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(false), cancellationToken);

        _logger.LogInformation("Product {ProductId} archived", product.Id);

        return new ArchiveProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}