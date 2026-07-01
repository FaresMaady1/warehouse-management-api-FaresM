namespace WebApi.Models;

public class Product
{
    private string Id;
    private string Name;
    private string SKU;
    private string Description;
    private decimal Price;
    private int QuantityInStock;
    private string supplierName;
    private DateTime ExpiryDate;
    private bool IsArchived;
    private DateTime CreatedAt;
    private DateTime LastUpdatedAt;
}