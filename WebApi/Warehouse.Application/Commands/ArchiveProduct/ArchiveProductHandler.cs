namespace Warehouse.Application.Commands.ArchiveProduct;

using MediatR;
using Warehouse.Domain.Repositories;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, ArchiveProductResponse?>
{
    private readonly IProductRepository _productRepository;
    public ArchiveProductHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<ArchiveProductResponse?> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<ArchiveProductResponse?>(null);

        product.Archive();

        return Task.FromResult<ArchiveProductResponse?>(new ArchiveProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
