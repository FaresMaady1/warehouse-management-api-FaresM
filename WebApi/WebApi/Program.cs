using System.Globalization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.MemoryStorage;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Minio;
using Minio.DataModel.Args;
using Serilog;
using StackExchange.Redis;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Application.Jobs;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Identity;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;
using Warehouse.Infrastructure.Caching;
using Warehouse.Infrastructure.Identity;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Storage;
using WebApi.HealthChecks;
using WebApi.Middleware;
using WebApi.Swagger;

var bootstrapConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(bootstrapConfiguration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build()));
    });
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
    builder.Services.AddScoped<ISupplierDocumentRepository, SupplierDocumentRepository>();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
    builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

    // Firebase Authentication
    var firebaseProjectId = builder.Configuration["Firebase:ProjectId"]!;
    var firebaseServiceAccountPath = builder.Configuration["Firebase:ServiceAccountKeyPath"]!;

    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseServiceAccountPath)
    });

    using var firebaseKeyHttpClient = new HttpClient();
    var firebaseJwksJson = await firebaseKeyHttpClient.GetStringAsync(
        "https://www.googleapis.com/service_accounts/v1/jwk/securetoken@system.gserviceaccount.com");
    var firebaseSigningKeys = new JsonWebKeySet(firebaseJwksJson).GetSigningKeys();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false; // keep "sub" as-is instead of remapping to a claim URI
            options.IncludeErrorDetails = builder.Environment.IsDevelopment();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
                ValidateAudience = true,
                ValidAudience = firebaseProjectId,
                ValidateLifetime = true,
                RoleClaimType = "role",
                IssuerSigningKeys = firebaseSigningKeys
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Error(context.Exception, "JWT authentication failed");
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    });

    builder.Services.AddScoped<IIdentityService, FirebaseIdentityService>();

    // MinIO object storage
    var minioEndpoint = builder.Configuration["Minio:Endpoint"]!;
    var minioAccessKey = builder.Configuration["Minio:AccessKey"]!;
    var minioSecretKey = builder.Configuration["Minio:SecretKey"]!;
    var minioUseSsl = builder.Configuration.GetValue<bool>("Minio:UseSSL");
    var minioBucketName = builder.Configuration["Minio:BucketName"]!;

    builder.Services.AddSingleton<IMinioClient>(_ => new MinioClient()
        .WithEndpoint(minioEndpoint)
        .WithCredentials(minioAccessKey, minioSecretKey)
        .WithSSL(minioUseSsl)
        .Build());

    builder.Services.AddScoped<IFileStorageService>(sp =>
        new MinioFileStorageService(sp.GetRequiredService<IMinioClient>(), minioBucketName));

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

    builder.Services.AddSingleton<ICacheService>(sp => new RedisCacheService(
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<IConnectionMultiplexer>(),
        redisInstanceName));

    // Health checks
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString, name: "postgres")
        .AddCheck<RedisRetryHealthCheck>("redis");

    builder.Services.AddHealthChecksUI(opts =>
    {
        opts.AddHealthCheckEndpoint("api", "/health");
    }).AddInMemoryStorage();

    // Background jobs
    builder.Services.AddHangfire(config => config.UseMemoryStorage());
    builder.Services.AddHangfireServer();
    builder.Services.AddScoped<ProductExpiryCheckJob>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<AcceptLanguageHeaderFilter>();

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste the Firebase ID token here. The 'Bearer ' prefix is added automatically."
        });

        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var minio = scope.ServiceProvider.GetRequiredService<IMinioClient>();
        var bucketExists = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(minioBucketName));
        if (!bucketExists)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(minioBucketName));
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseRequestLocalization();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHangfireDashboard("/hangfire");

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui";
        options.ApiPath = "/health-ui-api";
    });

    RecurringJob.AddOrUpdate<ProductExpiryCheckJob>(
        "product-expiry-check",
        job => job.RunAsync(CancellationToken.None),
        Cron.Daily);

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}