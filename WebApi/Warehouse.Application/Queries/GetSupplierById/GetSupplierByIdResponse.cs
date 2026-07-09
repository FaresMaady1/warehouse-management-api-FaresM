namespace Warehouse.Application.Queries.GetSupplierById;

public record GetSupplierByIdResponse(string Id, string Name, string Country, string ContactEmail, string PhoneNumber, bool IsActive);
