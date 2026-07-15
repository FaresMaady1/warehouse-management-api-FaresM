namespace Warehouse.Application.Commands.UploadProductImage;

using MediatR;
using Warehouse.Domain.ProductImages;
using Warehouse.Domain.Repositories;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, UploadProductImageResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;

    public UploadProductImageHandler(IProductRepository productRepository, IProductImageRepository productImageRepository)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
    }

    public async Task<UploadProductImageResponse?> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        Directory.CreateDirectory(Path.GetDirectoryName(request.FilePath)!);
        await File.WriteAllBytesAsync(request.FilePath, request.Content, cancellationToken);

        if (await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken) == null)
        {
            var image = ProductImage.Create(request.ProductId, request.FileName, request.FilePath);
            _productImageRepository.Add(image);
            await _productImageRepository.SaveChangesAsync(cancellationToken);
        }

        return new UploadProductImageResponse(request.FileName, request.FilePath);
    }
}