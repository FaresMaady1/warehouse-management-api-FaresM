namespace Warehouse.Application.Commands.Suppliers;

using MediatR;
using Warehouse.Domain.Suppliers;

public record CreateSupplierCommand(string Name, string Country, string ContactEmail, string PhoneNumber) : IRequest<Supplier>;
