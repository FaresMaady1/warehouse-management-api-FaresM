namespace Warehouse.Api.IntegrationTests;

using System.Net;
using FluentAssertions;
using Xunit;

[Collection("Integration Tests")]
public class SwaggerTests
{
    private readonly HttpClient _client;

    public SwaggerTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    // SwaggerJson
    [Fact]
    public async Task SwaggerJson_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}