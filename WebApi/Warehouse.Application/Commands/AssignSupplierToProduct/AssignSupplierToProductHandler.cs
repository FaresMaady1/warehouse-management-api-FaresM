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

    public Task<AssignSupplierToProductResponse?> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<AssignSupplierToProductResponse?>(null);

        var supplier = _supplierRepository.GetById(request.SupplierId);
        if (supplier == null) return Task.FromResult<AssignSupplierToProductResponse?>(null);

        product.AssignSupplier(supplier);
        _productRepository.SaveChanges();

        return Task.FromResult<AssignSupplierToProductResponse?>(new AssignSupplierToProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
