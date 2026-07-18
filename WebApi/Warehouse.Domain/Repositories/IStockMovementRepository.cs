namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.StockMovements;

public interface IStockMovementRepository : IRepository<StockMovement>
{
    Task<List<StockMovement>> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default);
    Task<List<StockMovement>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
}