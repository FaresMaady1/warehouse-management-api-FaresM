namespace WebApi.ViewModels;

public class ProductViewModel
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public string SupplierName { get; set; } = default!;
    public string? SupplierId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}