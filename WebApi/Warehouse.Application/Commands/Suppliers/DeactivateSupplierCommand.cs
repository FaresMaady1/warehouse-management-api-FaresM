namespace Warehouse.Application.Commands.Suppliers;

using MediatR;
using Warehouse.Domain.Suppliers;

public record DeactivateSupplierCommand(string SupplierId) : IRequest<Supplier?>;
