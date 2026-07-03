namespace WebApi.DB;
using WebApi.Models;
public class FakeSupplierStore
{
    public static List<Supplier> Suppliers = new List<Supplier>
    {
        new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TechSupply Co.",
            Country = "USA",
            ContactEmail = "contact@techsupply.com",
            PhoneNumber = "+1-555-0101",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Green Valley Farms",
            Country = "Lebanon",
            ContactEmail = "info@greenvalley.com",
            PhoneNumber = "+961-1-234567",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Golden Harvest Ltd.",
            Country = "India",
            ContactEmail = "sales@goldenharvest.com",
            PhoneNumber = "+91-22-1234567",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Mediterra Imports",
            Country = "Greece",
            ContactEmail = "hello@mediterra.com",
            PhoneNumber = "+30-21-0123456",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sunny Fields Beverages",
            Country = "Spain",
            ContactEmail = "contact@sunnyfields.com",
            PhoneNumber = "+34-91-1234567",
            IsActive = true
        }
    };
}