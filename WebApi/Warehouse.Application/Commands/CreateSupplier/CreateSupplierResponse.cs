namespace Warehouse.Application.Commands.CreateSupplier;

public record CreateSupplierResponse(string Id, string Name, string Country, string ContactEmail, string PhoneNumber, bool IsActive);
