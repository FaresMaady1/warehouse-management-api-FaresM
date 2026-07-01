namespace WebApi.Contracts;

public class CreateProductRequest
{
    public string Name;
    public string SKU;
    public string Description;
    public decimal Price;
    public int QuantityInStock;
    public string SupplierName;
    public DateTime ExpiryDate;
}