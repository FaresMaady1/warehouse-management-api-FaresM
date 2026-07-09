namespace Warehouse.Application.Commands.UploadProductImage;

using MediatR;
using Warehouse.Domain.Repositories;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, UploadProductImageResponse?>
{
    private readonly IProductRepository _productRepository;
    public UploadProductImageHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<UploadProductImageResponse?> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<UploadProductImageResponse?>(null);

        Directory.CreateDirectory(Path.GetDirectoryName(request.FilePath)!);
        File.WriteAllBytes(request.FilePath, request.Content);

        return Task.FromResult<UploadProductImageResponse?>(
            new UploadProductImageResponse(request.FileName, request.FilePath));
    }
}
