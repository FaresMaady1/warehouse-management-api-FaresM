using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Application.Preferences;
using Warehouse.Notifications.Application.Queries.ListNotifications;
using Warehouse.Notifications.Domain.Notifications;
using Warehouse.Notifications.Infrastructure.Messaging;
using Warehouse.Notifications.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("NotificationsDb")!;
builder.Services.AddDbContext<NotificationsDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ListNotificationsQuery).Assembly));

builder.Services.Configure<NotificationPreferences>(builder.Configuration.GetSection("NotificationPreferences"));

builder.Services.AddHostedService<WarehouseEventConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
