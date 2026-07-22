namespace Warehouse.Application.Jobs;

using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class ProductExpiryCheckJob
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<ProductExpiryCheckJob> _logger;

    public ProductExpiryCheckJob(IProductRepository productRepository, ICacheService cache, ILogger<ProductExpiryCheckJob> logger)
    {
        _productRepository = productRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var active = await _productRepository.GetActiveAsync(cancellationToken);
        var now = DateTime.Now;
        var expiringSoonCutoff = now.AddDays(30);
        var archiveCutoff = now.AddDays(-7);

        var expired = active.Where(p => p.ExpiryDate <= now).ToList();
        var expiringSoon = active.Where(p => p.ExpiryDate > now && p.ExpiryDate <= expiringSoonCutoff).ToList();
        var toArchive = active.Where(p => p.ExpiryDate <= archiveCutoff).ToList();

        _logger.LogInformation(
            "Product expiry check: {ExpiredCount} expired, {ExpiringSoonCount} expiring within 30 days",
            expired.Count, expiringSoon.Count);

        foreach (var product in expired)
            _logger.LogInformation("Expired product: {ProductName} ({ProductId}), expired {ExpiryDate}", product.Name, product.Id, product.ExpiryDate);

        foreach (var product in expiringSoon)
            _logger.LogInformation("Product expiring soon: {ProductName} ({ProductId}), expires {ExpiryDate}", product.Name, product.Id, product.ExpiryDate);

        if (toArchive.Count == 0) return;

        foreach (var product in toArchive)
        {
            product.Archive();
            _logger.LogInformation("Archived expired product: {ProductName} ({ProductId}), expired {ExpiryDate}", product.Name, product.Id, product.ExpiryDate);
            await _cache.RemoveAsync(CacheKeys.Product(product.Id), cancellationToken);
        }

        await _productRepository.SaveChangesAsync(cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(true), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(false), cancellationToken);
    }
}