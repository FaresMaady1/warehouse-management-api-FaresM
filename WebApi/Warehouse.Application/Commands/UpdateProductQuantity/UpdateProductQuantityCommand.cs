namespace Warehouse.Application.Commands.UpdateProductQuantity;

using MediatR;

public record UpdateProductQuantityCommand(string ProductId, int QuantityInStock) : IRequest<UpdateProductQuantityResponse?>;
