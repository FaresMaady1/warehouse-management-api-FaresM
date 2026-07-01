namespace WebApi.Models;

public class Product
{
    public string Id;
    public string Name;
    public string SKU;
    public string Description;
    public decimal Price;
    public int QuantityInStock;
    public string SupplierName;
    public DateTime ExpiryDate;
    public bool IsArchived;
    public DateTime CreatedAt;
    public DateTime LastUpdatedAt;
}