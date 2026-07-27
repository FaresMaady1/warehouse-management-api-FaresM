namespace Warehouse.Api.UnitTests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Application.Commands.AssignSupplierToProduct;
using Warehouse.Application.Commands.CreateSupplier;
using Warehouse.Application.Commands.DeactivateSupplier;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Suppliers;
using Xunit;

public class SupplierHandlersTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ICacheService> _cache = new();

    //Create supplier handler

    [Fact]
    public async Task CreateSupplier_ValidData_Succeeds()
    {
        var handler = new CreateSupplierHandler(_supplierRepository.Object, _cache.Object,
            Mock.Of<ILogger<CreateSupplierHandler>>());

        var result = await handler.Handle(
            new CreateSupplierCommand("Acme", "USA", "a@acme.com", "123"), default);

        result.Name.Should().Be("Acme");
        result.IsActive.Should().BeTrue();
        _supplierRepository.Verify(r => r.Add(It.IsAny<Supplier>()), Times.Once);
        _supplierRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // Deactivate supplier handler 

    [Fact]
    public async Task DeactivateSupplier_MarksInactive()
    {
        var supplier = Supplier.Create("Acme", "USA", "a@acme.com", "123");
        _supplierRepository.Setup(r => r.GetByIdAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);

        var handler = new DeactivateSupplierHandler(_supplierRepository.Object, _cache.Object,
            Mock.Of<ILogger<DeactivateSupplierHandler>>());

        var result = await handler.Handle(new DeactivateSupplierCommand(supplier.Id), default);

        result!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateSupplier_MissingSupplier_ReturnsNull()
    {
        _supplierRepository.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var handler = new DeactivateSupplierHandler(_supplierRepository.Object, _cache.Object,
            Mock.Of<ILogger<DeactivateSupplierHandler>>());

        var result = await handler.Handle(new DeactivateSupplierCommand("missing"), default);

        result.Should().BeNull();
    }

    //Assign supplier to product handler

    [Fact]
    public async Task AssignSupplier_ValidProductAndSupplier_Succeeds()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Unassigned", DateTime.Now.AddYears(1));
        var supplier = Supplier.Create("Acme", "USA", "a@acme.com", "123");

        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _supplierRepository.Setup(r => r.GetByIdAsync(supplier.Id, It.IsAny<CancellationToken>())).ReturnsAsync(supplier);

        var handler = new AssignSupplierToProductHandler(_productRepository.Object, _supplierRepository.Object, _cache.Object);
        var result = await handler.Handle(new AssignSupplierToProductCommand(product.Id, supplier.Id), default);

        result!.SupplierId.Should().Be(supplier.Id);
        result.SupplierName.Should().Be("Acme");
    }

    [Fact]
    public async Task AssignSupplier_ArchivedProduct_ThrowsDomainException()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Unassigned", DateTime.Now.AddYears(1));
        product.Archive();
        var supplier = Supplier.Create("Acme", "USA", "a@acme.com", "123");

        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _supplierRepository.Setup(r => r.GetByIdAsync(supplier.Id, It.IsAny<CancellationToken>())).ReturnsAsync(supplier);

        var handler = new AssignSupplierToProductHandler(_productRepository.Object, _supplierRepository.Object, _cache.Object);
        var act = () => handler.Handle(new AssignSupplierToProductCommand(product.Id, supplier.Id), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task AssignSupplier_MissingSupplier_ReturnsNull()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Unassigned", DateTime.Now.AddYears(1));

        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _supplierRepository.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>())).ReturnsAsync((Supplier?)null);

        var handler = new AssignSupplierToProductHandler(_productRepository.Object, _supplierRepository.Object, _cache.Object);
        var result = await handler.Handle(new AssignSupplierToProductCommand(product.Id, "missing"), default);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AssignSupplier_MissingProduct_ReturnsNull()
    {
        var supplier = Supplier.Create("Acme", "USA", "a@acme.com", "123");
        _productRepository.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new AssignSupplierToProductHandler(_productRepository.Object, _supplierRepository.Object, _cache.Object);
        var result = await handler.Handle(new AssignSupplierToProductCommand("missing", supplier.Id), default);

        result.Should().BeNull();
    }
}