namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly WarehouseDbContext Context;
    protected Repository(WarehouseDbContext context) => Context = context;

    public void Add(T entity) => Context.Set<T>().Add(entity);

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Context.SaveChangesAsync(cancellationToken);
}