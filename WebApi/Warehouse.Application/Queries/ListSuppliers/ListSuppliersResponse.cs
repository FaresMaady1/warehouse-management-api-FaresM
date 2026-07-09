namespace Warehouse.Application.Queries.ListSuppliers;

public record SupplierResponse(string Id, string Name, string Country, string ContactEmail, string PhoneNumber, bool IsActive);
