namespace Warehouse.Application.Commands.DeleteProductImage;

using MediatR;

public record DeleteProductImageCommand(string ProductId) : IRequest<bool>;