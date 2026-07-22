namespace Warehouse.Application.Commands.CreateProduct;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(IProductRepository productRepository, ICacheService cache, ILogger<CreateProductHandler> logger)
    {
        _productRepository = productRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _productRepository.SkuExistsAsync(request.SKU, cancellationToken))
            throw new DomainException($"A product with SKU '{request.SKU}' already exists.");

        var product = Product.Create(request.Name, request.SKU, request.Description,
            request.Price, request.QuantityInStock, request.SupplierName, request.ExpiryDate);

        _productRepository.Add(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(CacheKeys.ProductList(true), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(false), cancellationToken);

        _logger.LogInformation("Product {ProductId} ({Sku}) created", product.Id, product.SKU);

        return new CreateProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}