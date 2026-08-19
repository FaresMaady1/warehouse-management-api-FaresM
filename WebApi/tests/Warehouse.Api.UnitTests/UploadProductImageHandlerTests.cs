namespace Warehouse.Api.UnitTests;

using FluentAssertions;
using Moq;
using Warehouse.Application.Commands.UploadProductImage;
using Warehouse.Domain.Products;
using Warehouse.Domain.ProductImages;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;
using Xunit;

public class UploadProductImageHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IProductImageRepository> _productImageRepository = new();
    private readonly Mock<IFileStorageService> _storage = new();

    // UploadProductImageHandler
    [Fact]
    public async Task Handle_ValidUpload_GeneratesObjectKeyUnderProductFolder()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productImageRepository.Setup(r => r.GetByProductIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync((ProductImage?)null);

        var handler = new UploadProductImageHandler(_productRepository.Object, _productImageRepository.Object, _storage.Object);
        using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var result = await handler.Handle(new UploadProductImageCommand(product.Id, "photo.jpg", "image/jpeg", content), default);

        result!.ObjectKey.Should().StartWith($"products/{product.Id}/");
        result.ObjectKey.Should().EndWith("-photo.jpg");
    }

    // UploadProductImageHandler
    [Fact]
    public async Task Handle_ExistingImage_DeletesOldOneBeforeAddingNew()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        var existingImage = ProductImage.Create(product.Id, "old.jpg", "products/old-key.jpg", "image/jpeg");

        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productImageRepository.Setup(r => r.GetByProductIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingImage);

        var handler = new UploadProductImageHandler(_productRepository.Object, _productImageRepository.Object, _storage.Object);
        using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        await handler.Handle(new UploadProductImageCommand(product.Id, "new.jpg", "image/jpeg", content), default);

        _storage.Verify(s => s.DeleteAsync("products/old-key.jpg", It.IsAny<CancellationToken>()), Times.Once);
        _productImageRepository.Verify(r => r.Remove(existingImage), Times.Once);
        _productImageRepository.Verify(r => r.Add(It.IsAny<ProductImage>()), Times.Once);
    }

    // UploadProductImageHandler
    [Fact]
    public async Task Handle_MissingProduct_ReturnsNull()
    {
        _productRepository.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new UploadProductImageHandler(_productRepository.Object, _productImageRepository.Object, _storage.Object);
        using var content = new MemoryStream();

        var result = await handler.Handle(new UploadProductImageCommand("missing", "photo.jpg", "image/jpeg", content), default);

        result.Should().BeNull();
    }
}