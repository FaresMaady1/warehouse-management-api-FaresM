namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.StockMovements;

public class StockMovementRepository : Repository<StockMovement>, IStockMovementRepository
{
    public StockMovementRepository(WarehouseDbContext context) : base(context) { }

    public Task<List<StockMovement>> GetByProductIdAsync(string productId, CancellationToken cancellationToken = default) =>
        Context.Set<StockMovement>().Where(m => m.ProductId == productId).ToListAsync(cancellationToken);

    public Task<List<StockMovement>> GetRecentAsync(int count, CancellationToken cancellationToken = default) =>
        Context.Set<StockMovement>().OrderByDescending(m => m.OccurredAt).Take(count).ToListAsync(cancellationToken);
}