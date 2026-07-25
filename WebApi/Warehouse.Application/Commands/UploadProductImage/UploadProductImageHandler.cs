namespace Warehouse.Application.Commands.UploadProductImage;

using MediatR;
using Warehouse.Domain.ProductImages;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, UploadProductImageResponse?>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _storage;

    public UploadProductImageHandler(IProductRepository productRepository, IProductImageRepository productImageRepository, IFileStorageService storage)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _storage = storage;
    }

    public async Task<UploadProductImageResponse?> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        var objectKey = $"products/{request.ProductId}/{Guid.NewGuid()}-{request.FileName}";
        await _storage.UploadAsync(objectKey, request.Content, request.ContentType, cancellationToken);

        var existing = await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (existing != null)
        {
            await _storage.DeleteAsync(existing.ObjectKey, cancellationToken);
            _productImageRepository.Remove(existing);
        }

        _productImageRepository.Add(ProductImage.Create(request.ProductId, request.FileName, objectKey, request.ContentType));
        await _productImageRepository.SaveChangesAsync(cancellationToken);

        return new UploadProductImageResponse(request.FileName, objectKey);
    }
}