using System.Globalization;
using Hangfire;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Localization;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;
using WebApi.Middleware;
using WebApi.Swagger;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    var connectionString = builder.Configuration.GetConnectionString("WarehouseDb")!;
    builder.Services.AddDbContext<WarehouseDbContext>(options => options.UseNpgsql(connectionString));

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
    builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
    builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

    // Localization
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    var supportedCultures = new[] { "en", "fr", "ar" };
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    });
    
    builder.Services.AddSingleton<IStringLocalizer>(sp =>
    {
        var factory = sp.GetRequiredService<IStringLocalizerFactory>();
        var assemblyName = typeof(Program).Assembly.GetName().Name!;
        return factory.Create("SharedResources", assemblyName);
    });

    // Redis cache
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
    const string redisInstanceName = "WarehouseApi_";

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = redisInstanceName;
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        _ => ConnectionMultiplexer.Connect(redisConnectionString));

    

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => c.OperationFilter<AcceptLanguageHeaderFilter>());

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseRequestLocalization();
    
    app.MapControllers();

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}