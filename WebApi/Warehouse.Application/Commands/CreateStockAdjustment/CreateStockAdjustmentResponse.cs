namespace Warehouse.Application.Commands.CreateStockAdjustment;

public record CreateStockAdjustmentResponse(string MovementId, string ProductId, int QuantityChanged, string Reason, int NewQuantityInStock, DateTime OccurredAt);