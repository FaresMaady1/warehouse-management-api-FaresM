namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Products;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, Product?>
{
    private readonly IProductRepository _productRepository;
    public UploadProductImageHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product?> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<Product?>(null);

        Directory.CreateDirectory(Path.GetDirectoryName(request.FilePath)!);
        File.WriteAllBytes(request.FilePath, request.Content);

        return Task.FromResult<Product?>(product);
    }
}
