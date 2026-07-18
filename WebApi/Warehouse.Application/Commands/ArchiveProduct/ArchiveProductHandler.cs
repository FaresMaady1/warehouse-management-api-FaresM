namespace Warehouse.Application.Commands.ArchiveProduct;

using MediatR;
using Warehouse.Domain.Repositories;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, ArchiveProductResponse?>
{
    private readonly IProductRepository _productRepository;
    public ArchiveProductHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<ArchiveProductResponse?> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        product.Archive();
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ArchiveProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}