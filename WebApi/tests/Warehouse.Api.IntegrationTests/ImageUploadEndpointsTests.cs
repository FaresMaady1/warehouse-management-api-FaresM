namespace Warehouse.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WebApi.ViewModels;
using Xunit;

[Collection("Integration Tests")]
public class ImageUploadEndpointsTests
{
    private readonly HttpClient _client;

    public ImageUploadEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken("tester", "admin"));
    }

    private async Task<string> CreateTestProductIdAsync()
    {
        var request = new
        {
            Name = "Widget", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddYears(1)
        };
        var response = await _client.PostAsJsonAsync("/api/products", request);
        var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
        return product!.Id;
    }

    private static MultipartFormDataContent BuildFileContent(string fileName, string contentType, int sizeBytes = 100)
    {
        var fileContent = new ByteArrayContent(new byte[sizeBytes]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        return new MultipartFormDataContent { { fileContent, "file", fileName } };
    }

    // UploadImage
    [Fact]
    public async Task UploadImage_Jpg_Succeeds()
    {
        var productId = await CreateTestProductIdAsync();

        var response = await _client.PostAsync($"/api/products/{productId}/image", BuildFileContent("photo.jpg", "image/jpeg"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // UploadImage
    [Fact]
    public async Task UploadImage_Png_Succeeds()
    {
        var productId = await CreateTestProductIdAsync();

        var response = await _client.PostAsync($"/api/products/{productId}/image", BuildFileContent("photo.png", "image/png"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // UploadImage
    [Fact]
    public async Task UploadImage_TxtFile_Rejected()
    {
        var productId = await CreateTestProductIdAsync();

        var response = await _client.PostAsync($"/api/products/{productId}/image", BuildFileContent("notes.txt", "text/plain"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // UploadImage
    [Fact]
    public async Task UploadImage_OversizedFile_Rejected()
    {
        var productId = await CreateTestProductIdAsync();

        var response = await _client.PostAsync($"/api/products/{productId}/image",
            BuildFileContent("photo.jpg", "image/jpeg", sizeBytes: 3 * 1024 * 1024));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}