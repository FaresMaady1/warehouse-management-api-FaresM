namespace Warehouse.Application.Commands.AssignSupplierToProduct;

using MediatR;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Repositories;

public class AssignSupplierToProductHandler : IRequestHandler<AssignSupplierToProductCommand, AssignSupplierToProductResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICacheService _cache;

    public AssignSupplierToProductHandler(IProductRepository productRepository, ISupplierRepository supplierRepository, ICacheService cache)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _cache = cache;
    }

    public async Task<AssignSupplierToProductResponse?> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) return null;

        product.AssignSupplier(supplier);
        await _productRepository.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"products:{product.Id}", cancellationToken);
        await _cache.RemoveAsync("products:list:True", cancellationToken);
        await _cache.RemoveAsync("products:list:False", cancellationToken);

        return new AssignSupplierToProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}