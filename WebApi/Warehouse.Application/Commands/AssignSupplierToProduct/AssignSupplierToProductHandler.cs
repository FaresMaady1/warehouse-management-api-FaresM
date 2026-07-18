namespace Warehouse.Application.Commands.AssignSupplierToProduct;

using MediatR;
using Warehouse.Domain.Repositories;

public class AssignSupplierToProductHandler : IRequestHandler<AssignSupplierToProductCommand, AssignSupplierToProductResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierToProductHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<AssignSupplierToProductResponse?> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) return null;

        product.AssignSupplier(supplier);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new AssignSupplierToProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}