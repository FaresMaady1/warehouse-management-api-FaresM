namespace WebApi.DB;
using WebApi.Models;

public class FakeWarehouseStore
{
          private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301").ToString(),
                Name = "Wireless Mouse",
                SKU = "ELEC-001",
                Description = "Ergonomic wireless mouse with USB receiver",
                Price = 19.99m,
                QuantityInStock = 150,
                SupplierName = "TechSupply Co.",
                ExpiryDate = DateTime.Now.AddYears(5)
            },
            new Product
            {
                Id = Guid.Parse("2b60a60d-e313-4454-9d9f-a1739f58aa87").ToString(),
                Name = "Mechanical Keyboard",
                SKU = "ELEC-002",
                Description = "RGB backlit mechanical keyboard",
                Price = 59.99m,
                QuantityInStock = 80,
                SupplierName = "TechSupply Co.",
                ExpiryDate = DateTime.Now.AddYears(5)
            },
            new Product
            {
                Id = Guid.Parse("86451dd2-da87-4362-a186-5e04aa125afb").ToString(),
                Name = "Whole Milk",
                SKU = "DAIRY-001",
                Description = "1 gallon whole milk",
                Price = 3.49m,
                QuantityInStock = 200,
                SupplierName = "Green Valley Farms",
                ExpiryDate = DateTime.Now.AddDays(14)
            },
            new Product
            {
                Id = Guid.Parse("c376238a-2723-4ea1-bb48-9298a27a58f2").ToString(),
                Name = "Cheddar Cheese",
                SKU = "DAIRY-002",
                Description = "Sharp cheddar cheese block, 500g",
                Price = 5.99m,
                QuantityInStock = 120,
                SupplierName = "Green Valley Farms",
                ExpiryDate = DateTime.Now.AddDays(60)
            },
            new Product
            {
                Id = Guid.Parse("d21df4a5-cd5f-4345-9bcc-1e018bef411f").ToString(),
                Name = "Basmati Rice",
                SKU = "GRAIN-001",
                Description = "5kg bag of premium basmati rice",
                Price = 12.99m,
                QuantityInStock = 300,
                SupplierName = "Golden Harvest Ltd.",
                ExpiryDate = DateTime.Now.AddMonths(18)
            },
            new Product
            {
                Id = Guid.Parse("acdac899-bbf3-4563-9c15-d25064255fbc").ToString(),
                Name = "Olive Oil",
                SKU = "OIL-001",
                Description = "Extra virgin olive oil, 1L",
                Price = 9.99m,
                QuantityInStock = 90,
                SupplierName = "Mediterra Imports",
                ExpiryDate = DateTime.Now.AddMonths(24)
            },
            new Product
            {
                Id = Guid.Parse("04d8c4e0-0a74-4172-9805-a02d6aa202d2").ToString(),
                Name = "Bluetooth Speaker",
                SKU = "ELEC-003",
                Description = "Portable Bluetooth speaker with 10h battery",
                Price = 39.99m,
                QuantityInStock = 60,
                SupplierName = "TechSupply Co.",
                ExpiryDate = DateTime.Now.AddYears(5)
            },
            new Product
            {
                Id = Guid.Parse("59259ca4-3749-44be-8455-ce4392577fc1").ToString(),
                SKU = "BEV-001",
                Description = "100% pure orange juice, 1L",
                Price = 4.29m,
                QuantityInStock = 175,
                SupplierName = "Sunny Fields Beverages",
                ExpiryDate = DateTime.Now.AddDays(21)
            },
            new Product
            {
                Id = Guid.Parse("f21a8cb4-5565-474e-8ceb-dd1a123ed6c0").ToString(),
                Name = "Pasta",
                SKU = "GRAIN-002",
                Description = "Durum wheat pasta, 1kg",
                Price = 2.49m,
                QuantityInStock = 250,
                SupplierName = "Golden Harvest Ltd.",
                ExpiryDate = DateTime.Now.AddMonths(12)
            },
            new Product
            {
                Id = Guid.Parse("0d18b21e-20cf-4b83-9424-9c2932ad5787").ToString(),
                Name = "Green Tea",
                SKU = "BEV-002",
                Description = "Organic green tea, box of 50 bags",
                Price = 6.49m,
                QuantityInStock = 140,
                SupplierName = "Mediterra Imports",
                ExpiryDate = DateTime.Now.AddMonths(18)
            }
        };
          
}