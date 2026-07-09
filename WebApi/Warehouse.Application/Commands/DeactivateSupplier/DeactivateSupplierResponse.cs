namespace Warehouse.Application.Commands.DeactivateSupplier;

public record DeactivateSupplierResponse(string Id, string Name, string Country, string ContactEmail, string PhoneNumber, bool IsActive);
