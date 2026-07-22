namespace Warehouse.Application.Queries.DownloadProductImage;

using MediatR;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;

public class DownloadProductImageHandler : IRequestHandler<DownloadProductImageQuery, DownloadProductImageResponse?>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _storage;

    public DownloadProductImageHandler(IProductImageRepository productImageRepository, IFileStorageService storage)
    {
        _productImageRepository = productImageRepository;
        _storage = storage;
    }

    public async Task<DownloadProductImageResponse?> Handle(DownloadProductImageQuery request, CancellationToken cancellationToken)
    {
        var image = await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (image == null) return null;

        var stream = await _storage.DownloadAsync(image.ObjectKey, cancellationToken);
        return new DownloadProductImageResponse(image.FileName, image.ContentType, stream);
    }
}