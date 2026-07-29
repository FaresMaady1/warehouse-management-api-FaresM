namespace Warehouse.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.ViewModels;
using Xunit;

[Collection("Integration Tests")]
public class SupplierEndpointsTests
{
    private readonly HttpClient _client;

    public SupplierEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken("tester", "admin"));
    }

    private async Task<SupplierViewModel> CreateTestSupplierAsync()
    {
        var request = new { Name = "Test Supplier", Country = "USA", ContactEmail = "test@supplier.com", PhoneNumber = "123" };
        var response = await _client.PostAsJsonAsync("/api/suppliers", request);
        return (await response.Content.ReadFromJsonAsync<SupplierViewModel>())!;
    }

    // Create
    [Fact]
    public async Task Create_ValidSupplier_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/suppliers",
            new { Name = "New Supplier", Country = "USA", ContactEmail = "new@supplier.com", PhoneNumber = "123" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // GetById
    [Fact]
    public async Task GetById_ReturnsSupplier()
    {
        var supplier = await CreateTestSupplierAsync();

        var response = await _client.GetAsync($"/api/suppliers/{supplier.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var found = await response.Content.ReadFromJsonAsync<SupplierViewModel>();
        found!.Id.Should().Be(supplier.Id);
    }

    // Deactivate
    [Fact]
    public async Task Deactivate_MarksInactive()
    {
        var supplier = await CreateTestSupplierAsync();

        var response = await _client.DeleteAsync($"/api/suppliers/{supplier.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var deactivated = await response.Content.ReadFromJsonAsync<SupplierViewModel>();
        deactivated!.IsActive.Should().BeFalse();
    }

    // AssignSupplierToProduct
    [Fact]
    public async Task AssignSupplierToProduct_Works()
    {
        var supplier = await CreateTestSupplierAsync();
        var productResponse = await _client.PostAsJsonAsync("/api/products", new
        {
            Name = "Widget", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Unassigned", ExpiryDate = DateTime.Now.AddYears(1)
        });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductViewModel>();

        var response = await _client.PostAsync($"/api/products/{product!.Id}/assign-supplier/{supplier.Id}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        updated!.SupplierId.Should().Be(supplier.Id);
    }
}