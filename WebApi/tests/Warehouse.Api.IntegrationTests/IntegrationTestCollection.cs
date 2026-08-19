namespace Warehouse.Api.IntegrationTests;

using Xunit;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}