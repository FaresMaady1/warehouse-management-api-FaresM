namespace Warehouse.Application.Commands.DeleteProductImage;

using MediatR;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;

public class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand, bool>
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _storage;

    public DeleteProductImageHandler(IProductImageRepository productImageRepository, IFileStorageService storage)
    {
        _productImageRepository = productImageRepository;
        _storage = storage;
    }

    public async Task<bool> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (image == null) return false;

        await _storage.DeleteAsync(image.ObjectKey, cancellationToken);
        _productImageRepository.Remove(image);
        await _productImageRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}