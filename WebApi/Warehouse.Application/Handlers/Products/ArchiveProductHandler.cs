namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Products;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, Product?>
{
    private readonly IProductRepository _productRepository;
    public ArchiveProductHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product?> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<Product?>(null);

        product.Archive();
        return Task.FromResult<Product?>(product);
    }
}
