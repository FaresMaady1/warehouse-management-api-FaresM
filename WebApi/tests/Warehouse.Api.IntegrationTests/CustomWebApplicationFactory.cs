namespace Warehouse.Api.IntegrationTests;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Storage;
using Warehouse.Infrastructure.Persistence;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"WarehouseTestDb-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<WarehouseDbContext>));
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.RemoveAll(typeof(ICacheService));
            services.AddSingleton<ICacheService, FakeCacheService>();

            services.RemoveAll(typeof(IFileStorageService));
            services.AddSingleton<IFileStorageService, FakeFileStorageService>();

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidateIssuer = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.IssuerSigningKey = TestJwt.SigningKey;
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
            db.Database.EnsureCreated();
        });
    }
}