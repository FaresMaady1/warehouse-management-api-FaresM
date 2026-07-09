namespace Warehouse.Application.Commands.AssignSupplierToProduct;

using MediatR;

public record AssignSupplierToProductCommand(string ProductId, string SupplierId) : IRequest<AssignSupplierToProductResponse?>;
