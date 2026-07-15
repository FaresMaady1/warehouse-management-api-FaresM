namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Products;
using Warehouse.Domain.Suppliers;
using Warehouse.Domain.ProductImages;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.ExpiryDate).HasColumnType("timestamp without time zone");
            entity.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
            entity.Property(p => p.LastUpdatedAt).HasColumnType("timestamp without time zone");
            entity.HasOne<Supplier>()
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .IsRequired(false);
        });

        modelBuilder.Entity<Supplier>(entity => entity.HasKey(s => s.Id));

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(pi => pi.ProductId);
            entity.HasOne<Product>()
                .WithOne()
                .HasForeignKey<ProductImage>(pi => pi.ProductId);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // fixed date, not DateTime.Now — HasData needs deterministic values for the migration
        var seedDate = new DateTime(2026, 1, 1);

        modelBuilder.Entity<Supplier>().HasData(
            Supplier.Restore("72338e71-fe24-44d6-a6ae-5396bd2ce8bb", "TechSupply Co.", "USA", "contact@techsupply.com", "+1-555-0101", true),
            Supplier.Restore("6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", "Green Valley Farms", "Lebanon", "info@greenvalley.com", "+961-1-234567", true),
            Supplier.Restore("8ac4d89a-bef3-47c8-883f-ecade15fd80f", "Golden Harvest Ltd.", "India", "sales@goldenharvest.com", "+91-22-1234567", true),
            Supplier.Restore("d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", "Mediterra Imports", "Greece", "hello@mediterra.com", "+30-21-0123456", true),
            Supplier.Restore("b54f534a-3d73-46bd-95a4-edf561d36ab2", "Sunny Fields Beverages", "Spain", "contact@sunnyfields.com", "+34-91-1234567", true)
        );

        modelBuilder.Entity<Product>().HasData(
            Product.Restore("3f2504e0-4f89-11d3-9a0c-0305e82c3301", "Wireless Mouse", "ELEC-001", "Ergonomic wireless mouse with USB receiver", 19.99m, 150, "TechSupply Co.", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", seedDate.AddYears(5), false, seedDate, seedDate),
            Product.Restore("2b60a60d-e313-4454-9d9f-a1739f58aa87", "Mechanical Keyboard", "ELEC-002", "RGB backlit mechanical keyboard", 59.99m, 80, "TechSupply Co.", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", seedDate.AddYears(5), false, seedDate, seedDate),
            Product.Restore("86451dd2-da87-4362-a186-5e04aa125afb", "Whole Milk", "DAIRY-001", "1 gallon whole milk", 3.49m, 200, "Green Valley Farms", "6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", seedDate.AddDays(14), false, seedDate, seedDate),
            Product.Restore("c376238a-2723-4ea1-bb48-9298a27a58f2", "Cheddar Cheese", "DAIRY-002", "Sharp cheddar cheese block, 500g", 5.99m, 120, "Green Valley Farms", "6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", seedDate.AddDays(60), false, seedDate, seedDate),
            Product.Restore("d21df4a5-cd5f-4345-9bcc-1e018bef411f", "Basmati Rice", "GRAIN-001", "5kg bag of premium basmati rice", 12.99m, 300, "Golden Harvest Ltd.", "8ac4d89a-bef3-47c8-883f-ecade15fd80f", seedDate.AddMonths(18), false, seedDate, seedDate),
            Product.Restore("acdac899-bbf3-4563-9c15-d25064255fbc", "Olive Oil", "OIL-001", "Extra virgin olive oil, 1L", 9.99m, 90, "Mediterra Imports", "d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", seedDate.AddMonths(24), false, seedDate, seedDate),
            Product.Restore("04d8c4e0-0a74-4172-9805-a02d6aa202d2", "Bluetooth Speaker", "ELEC-003", "Portable Bluetooth speaker with 10h battery", 39.99m, 60, "TechSupply Co.", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", seedDate.AddYears(5), false, seedDate, seedDate),
            Product.Restore("59259ca4-3749-44be-8455-ce4392577fc1", "Orange Juice", "BEV-001", "100% pure orange juice, 1L", 4.29m, 175, "Sunny Fields Beverages", "b54f534a-3d73-46bd-95a4-edf561d36ab2", seedDate.AddDays(21), false, seedDate, seedDate),
            Product.Restore("f21a8cb4-5565-474e-8ceb-dd1a123ed6c0", "Pasta", "GRAIN-002", "Durum wheat pasta, 1kg", 2.49m, 250, "Golden Harvest Ltd.", "8ac4d89a-bef3-47c8-883f-ecade15fd80f", seedDate.AddMonths(12), false, seedDate, seedDate),
            Product.Restore("0d18b21e-20cf-4b83-9424-9c2932ad5787", "Green Tea", "BEV-002", "Organic green tea, box of 50 bags", 6.49m, 140, "Mediterra Imports", "d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", seedDate.AddMonths(18), false, seedDate, seedDate)
        );
    }
}