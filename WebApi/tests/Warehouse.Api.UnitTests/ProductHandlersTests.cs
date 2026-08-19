namespace Warehouse.Api.UnitTests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Application.Commands.ArchiveProduct;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Application.Commands.UpdateProductPrice;
using Warehouse.Application.Commands.UpdateProductQuantity;
using Warehouse.Application.Queries.SearchProducts;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;
using Xunit;

public class ProductHandlersTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICacheService> _cache = new();

    // Create product handler 

    [Fact]
    public async Task CreateProduct_ValidData_Succeeds()
    {
        _productRepository.Setup(r => r.SkuExistsAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreateProductHandler(_productRepository.Object, _cache.Object,
            Mock.Of<ILogger<CreateProductHandler>>());

        var command = new CreateProductCommand("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        var result = await handler.Handle(command, default);

        result.Name.Should().Be("Mouse");
        result.Id.Should().NotBeNullOrEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        _productRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _productRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_DuplicateSku_ThrowsDomainException()
    {
        _productRepository.Setup(r => r.SkuExistsAsync("SKU-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateProductHandler(_productRepository.Object, _cache.Object,
            Mock.Of<ILogger<CreateProductHandler>>());

        var command = new CreateProductCommand("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        var act = () => handler.Handle(command, default);

        await act.Should().ThrowAsync<DomainException>();
        _productRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    //Update product quantity handler 

    [Fact]
    public async Task UpdateQuantity_ValidQuantity_UpdatesStock()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(_productRepository.Object, _cache.Object);
        var result = await handler.Handle(new UpdateProductQuantityCommand(product.Id, 20), default);

        result!.QuantityInStock.Should().Be(20);
        _productRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateQuantity_NegativeQuantity_ThrowsDomainException()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductQuantityHandler(_productRepository.Object, _cache.Object);
        var act = () => handler.Handle(new UpdateProductQuantityCommand(product.Id, -5), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task UpdateQuantity_MissingProduct_ReturnsNull()
    {
        _productRepository.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductQuantityHandler(_productRepository.Object, _cache.Object);
        var result = await handler.Handle(new UpdateProductQuantityCommand("missing", 10), default);

        result.Should().BeNull();
    }

    // Update product price handler 

    [Fact]
    public async Task UpdatePrice_ValidPrice_Updates()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductPriceHandler(_productRepository.Object, _cache.Object);
        var result = await handler.Handle(new UpdateProductPriceCommand(product.Id, 25m), default);

        result!.Price.Should().Be(25m);
    }

    [Fact]
    public async Task UpdatePrice_InvalidPrice_ThrowsDomainException()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductPriceHandler(_productRepository.Object, _cache.Object);
        var act = () => handler.Handle(new UpdateProductPriceCommand(product.Id, 0m), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    // Archive product handler

    [Fact]
    public async Task ArchiveProduct_MarksArchivedOnly()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new ArchiveProductHandler(_productRepository.Object, _cache.Object,
            Mock.Of<ILogger<ArchiveProductHandler>>());

        var result = await handler.Handle(new ArchiveProductCommand(product.Id), default);

        result!.IsArchived.Should().BeTrue();
        result.Name.Should().Be("Mouse");
        result.QuantityInStock.Should().Be(5);
    }

    // Search products handler

    [Fact]
    public async Task Search_ForwardsNameAndSupplierToRepository()
    {
        _productRepository.Setup(r => r.SearchAsync("Mouse", "Acme", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var handler = new SearchProductsHandler(_productRepository.Object);
        await handler.Handle(new SearchProductsQuery("Mouse", "Acme"), default);

        _productRepository.Verify(r => r.SearchAsync("Mouse", "Acme", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Search_MapsProductsToResponses()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Acme", DateTime.Now.AddYears(1));
        _productRepository.Setup(r => r.SearchAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });

        var handler = new SearchProductsHandler(_productRepository.Object);
        var result = await handler.Handle(new SearchProductsQuery(null, null), default);

        result.Should().HaveCount(1);
        result[0].SKU.Should().Be("SKU-1");
    }
}