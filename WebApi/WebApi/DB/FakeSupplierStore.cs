namespace WebApi.DB;
using WebApi.Models;
public class FakeSupplierStore
{
    public static List<Supplier> Suppliers = new List<Supplier>
    {
        new Supplier
        {
            Id = Guid.Parse("72338e71-fe24-44d6-a6ae-5396bd2ce8bb").ToString(),
            Name = "TechSupply Co.",
            Country = "USA",
            ContactEmail = "contact@techsupply.com",
            PhoneNumber = "+1-555-0101",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.Parse("6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5").ToString(),
            Name = "Green Valley Farms",
            Country = "Lebanon",
            ContactEmail = "info@greenvalley.com",
            PhoneNumber = "+961-1-234567",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.Parse("8ac4d89a-bef3-47c8-883f-ecade15fd80f").ToString(),
            Name = "Golden Harvest Ltd.",
            Country = "India",
            ContactEmail = "sales@goldenharvest.com",
            PhoneNumber = "+91-22-1234567",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.Parse("d9bee142-bbf4-4ed6-a398-f6da24f7bfc3").ToString(),
            Name = "Mediterra Imports",
            Country = "Greece",
            ContactEmail = "hello@mediterra.com",
            PhoneNumber = "+30-21-0123456",
            IsActive = true
        },
        new Supplier
        {
            Id = Guid.Parse("b54f534a-3d73-46bd-95a4-edf561d36ab2").ToString(),
            Name = "Sunny Fields Beverages",
            Country = "Spain",
            ContactEmail = "contact@sunnyfields.com",
            PhoneNumber = "+34-91-1234567",
            IsActive = true
        }
    };
}