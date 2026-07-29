namespace Warehouse.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.ViewModels;
using Xunit;

[Collection("Integration Tests")]
public class FullBusinessFlowTests
{
    private readonly HttpClient _client;

    public FullBusinessFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken("tester", "admin"));
    }

    // FullLifecycle
    [Fact]
    public async Task FullLifecycle_Works()
    {
        var supplierResponse = await _client.PostAsJsonAsync("/api/suppliers",
            new { Name = "Flow Supplier", Country = "USA", ContactEmail = "flow@supplier.com", PhoneNumber = "123" });
        var supplier = await supplierResponse.Content.ReadFromJsonAsync<SupplierViewModel>();

        var productResponse = await _client.PostAsJsonAsync("/api/products", new
        {
            Name = "Flow Widget", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Unassigned", ExpiryDate = DateTime.Now.AddYears(1)
        });
        var product = await productResponse.Content.ReadFromJsonAsync<ProductViewModel>();

        var assignResponse = await _client.PostAsync($"/api/products/{product!.Id}/assign-supplier/{supplier!.Id}", null);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fileContent = new ByteArrayContent(new byte[100]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        var form = new MultipartFormDataContent { { fileContent, "file", "photo.jpg" } };
        var uploadResponse = await _client.PostAsync($"/api/products/{product.Id}/image", form);
        uploadResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var quantityResponse = await _client.PostAsJsonAsync($"/api/products/{product.Id}/quantity", new { QuantityInStock = 25 });
        quantityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var priceResponse = await _client.PostAsJsonAsync($"/api/products/{product.Id}/price", new { Price = 49.99m });
        priceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var archiveResponse = await _client.DeleteAsync($"/api/products/{product.Id}");
        archiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var finalResponse = await _client.GetAsync($"/api/products/{product.Id}");
        var final = await finalResponse.Content.ReadFromJsonAsync<ProductViewModel>();

        final!.IsArchived.Should().BeTrue();
        final.SupplierId.Should().Be(supplier.Id);
        final.QuantityInStock.Should().Be(25);
        final.Price.Should().Be(49.99m);
    }
}