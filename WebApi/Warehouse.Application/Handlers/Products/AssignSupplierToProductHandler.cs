namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Products;
using Warehouse.Domain.Suppliers;

public class AssignSupplierToProductHandler : IRequestHandler<AssignSupplierToProductCommand, Product?>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierToProductHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public Task<Product?> Handle(AssignSupplierToProductCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<Product?>(null);

        var supplier = _supplierRepository.GetById(request.SupplierId);
        if (supplier == null) return Task.FromResult<Product?>(null);

        product.AssignSupplier(supplier);
        return Task.FromResult<Product?>(product);
    }
}
