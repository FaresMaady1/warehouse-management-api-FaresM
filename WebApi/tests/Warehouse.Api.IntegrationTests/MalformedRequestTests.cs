namespace Warehouse.Api.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Xunit;

[Collection("Integration Tests")]
public class MalformedRequestTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MalformedRequestTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken("tester", "admin"));
    }

    // GetById
    [Fact]
    public async Task GetById_NonGuidId_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/products/not-a-guid");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Create
    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/products", new
        {
            Name = "", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddYears(1)
        });

        // caught by Product.Create's own guard, not the DTO's [Required] — Program.cs sets
        // SuppressModelStateInvalidFilter = true, so data-annotation validation never runs
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Create
    [Fact]
    public async Task Create_PastExpiryDate_IsNotRejected()
    {
        var response = await _client.PostAsJsonAsync("/api/products", new
        {
            Name = "Old Widget", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddDays(-1)
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // Create
    [Fact]
    public async Task Create_InvalidJsonBody_DoesNotSucceed()
    {
        var content = new StringContent("{ this is not json", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/products", content);
        
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    // Authorization
    [Fact]
    public async Task NoAuthorizationHeader_ReturnsUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Authorization
    [Fact]
    public async Task NonAdminToken_CreateProduct_ReturnsForbidden()
    {
        var nonAdminClient = _factory.CreateClient();
        nonAdminClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken("tester", "user"));

        var response = await nonAdminClient.PostAsJsonAsync("/api/products", new
        {
            Name = "Widget", SKU = $"SKU-{Guid.NewGuid()}", Description = "desc", Price = 10m,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddYears(1)
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}