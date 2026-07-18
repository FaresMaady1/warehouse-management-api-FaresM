namespace Warehouse.Domain.Tests;

using Warehouse.Application.Queries.GetProductById;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;
using Xunit;

public class GetProductByIdHandlerTests
{
    private class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_products);

        public Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<List<Product>> SearchAsync(string? name, string? supplier, CancellationToken cancellationToken = default) =>
            Task.FromResult(_products);

        public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default) =>
            Task.FromResult(_products.Any(p => p.SKU == sku));

        public void Add(Product product) => _products.Add(product);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public async Task Handle_Returns_Null_When_Product_Missing()
    {
        var handler = new GetProductByIdHandler(new FakeProductRepository());
        var result = await handler.Handle(new GetProductByIdQuery("missing-id"), default);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_Returns_Product_When_Found()
    {
        var repo = new FakeProductRepository();
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Supplier", DateTime.Now.AddYears(1));
        repo.Add(product);

        var handler = new GetProductByIdHandler(repo);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), default);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
    }
}