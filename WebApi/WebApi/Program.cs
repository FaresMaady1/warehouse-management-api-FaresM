using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();    
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WarehouseDb")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();

app.MapControllers();

app.Run();