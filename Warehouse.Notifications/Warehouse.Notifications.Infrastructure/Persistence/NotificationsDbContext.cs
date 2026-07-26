namespace Warehouse.Notifications.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Notifications.Domain.Notifications;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasIndex(n => n.SourceEventId).IsUnique();
            entity.Property(n => n.Type).HasMaxLength(50);
            entity.Property(n => n.Severity).HasMaxLength(20);
            entity.Property(n => n.Title).HasMaxLength(200);
            entity.Property(n => n.Message).HasMaxLength(1000);
        });
    }
}
