namespace Warehouse.Application.Commands.DeactivateSupplier;

using MediatR;

public record DeactivateSupplierCommand(string SupplierId) : IRequest<DeactivateSupplierResponse?>;
