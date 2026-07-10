namespace Warehouse.Domain.Products;

using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Suppliers;

public class Product
{
    public string Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string SKU { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int QuantityInStock { get; private set; }
    public string SupplierName { get; private set; } = default!;
    public string? SupplierId { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private Product() { }

    public static Product Create(string name, string sku, string description, decimal price,
        int quantityInStock, string supplierName, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero.");
        if (quantityInStock < 0)
            throw new DomainException("Quantity cannot be negative.");

        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            SKU = sku,
            Description = description,
            Price = price,
            QuantityInStock = quantityInStock,
            SupplierName = supplierName,
            ExpiryDate = expiryDate,
            IsArchived = false,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };
    }

    public static Product Restore(string id, string name, string sku, string description, decimal price,
        int quantityInStock, string supplierName, string? supplierId, DateTime expiryDate,
        bool isArchived, DateTime createdAt, DateTime lastUpdatedAt)
    {
        return new Product
        {
            Id = id,
            Name = name,
            SKU = sku,
            Description = description,
            Price = price,
            QuantityInStock = quantityInStock,
            SupplierName = supplierName,
            SupplierId = supplierId,
            ExpiryDate = expiryDate,
            IsArchived = isArchived,
            CreatedAt = createdAt,
            LastUpdatedAt = lastUpdatedAt
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (IsArchived)
            throw new DomainException("Cannot update an archived product.");
        if (quantity < 0)
            throw new DomainException("Quantity cannot be negative.");

        QuantityInStock = quantity;
        LastUpdatedAt = DateTime.Now;
    }

    public void UpdatePrice(decimal price)
    {
        if (IsArchived)
            throw new DomainException("Cannot update an archived product.");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero.");

        Price = price;
        LastUpdatedAt = DateTime.Now;
    }

    public void Archive()
    {
        IsArchived = true;
        LastUpdatedAt = DateTime.Now;
    }

    public void AssignSupplier(Supplier supplier)
    {
        if (IsArchived)
            throw new DomainException("Cannot assign a supplier to an archived product.");
        if (!supplier.IsActive)
            throw new DomainException("Cannot assign an inactive supplier.");

        SupplierId = supplier.Id;
        SupplierName = supplier.Name;
        LastUpdatedAt = DateTime.Now;
    }
}
