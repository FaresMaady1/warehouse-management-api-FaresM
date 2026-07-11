using System;
using System.Collections.Generic;

namespace WebApi.DbFirst.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public Guid? SupplierId { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual ProductImage? ProductImage { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
