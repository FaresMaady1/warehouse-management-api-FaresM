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

    public Task<UploadProductImageResponse?> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<UploadProductImageResponse?>(null);

        Directory.CreateDirectory(Path.GetDirectoryName(request.FilePath)!);
        File.WriteAllBytes(request.FilePath, request.Content);

        if (_productImageRepository.GetByProductId(request.ProductId) == null)
        {
            var image = ProductImage.Create(request.ProductId, request.FileName, request.FilePath);
            _productImageRepository.Add(image);
            _productImageRepository.SaveChanges();
        }

        return Task.FromResult<UploadProductImageResponse?>(
            new UploadProductImageResponse(request.FileName, request.FilePath));
    }
}