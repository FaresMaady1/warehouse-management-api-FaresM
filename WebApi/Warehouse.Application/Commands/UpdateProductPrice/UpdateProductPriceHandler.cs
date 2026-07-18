namespace Warehouse.Application.Commands.UpdateProductPrice;

using MediatR;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;

    public UpdateProductPriceHandler(IProductRepository productRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<UpdateProductPriceResponse?> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        product.UpdatePrice(request.Price);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"products:{product.Id}", cancellationToken);
        await _cache.RemoveAsync("products:list:True", cancellationToken);
        await _cache.RemoveAsync("products:list:False", cancellationToken);

        return new UpdateProductPriceResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}