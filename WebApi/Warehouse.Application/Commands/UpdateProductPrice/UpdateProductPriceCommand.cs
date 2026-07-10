namespace Warehouse.Application.Commands.UpdateProductPrice;

using MediatR;

public record UpdateProductPriceCommand(string ProductId, decimal Price) : IRequest<UpdateProductPriceResponse?>;
