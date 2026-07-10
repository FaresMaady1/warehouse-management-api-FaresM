namespace Warehouse.Application.Commands.CreateSupplier;

using MediatR;

public record CreateSupplierCommand(string Name, string Country, string ContactEmail, string PhoneNumber) : IRequest<CreateSupplierResponse>;
