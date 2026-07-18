namespace Warehouse.Application.Commands.CreateStockAdjustment;

using MediatR;

public record CreateStockAdjustmentCommand(string ProductId, int QuantityChanged, string Reason) : IRequest<CreateStockAdjustmentResponse>;