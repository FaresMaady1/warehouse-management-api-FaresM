namespace Warehouse.Application.Commands.UpdateProductQuantity;

using MediatR;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;

    public UpdateProductQuantityHandler(IProductRepository productRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<UpdateProductQuantityResponse?> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        product.UpdateQuantity(request.QuantityInStock);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"products:{product.Id}", cancellationToken);
        await _cache.RemoveAsync("products:list:True", cancellationToken);
        await _cache.RemoveAsync("products:list:False", cancellationToken);

        return new UpdateProductQuantityResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}