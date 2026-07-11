using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Domain.Repositories;
using Warehouse.Infrastructure.Persistence;
using WebApi.DbFirst;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WarehouseDbFirstContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WarehouseDbFirst")));
builder.Services.AddControllers();

builder.Services.AddSingleton<WarehouseDbContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
