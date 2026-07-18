namespace Warehouse.Domain.Repositories;

public interface IRepository<T>
{
    void Add(T entity);
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}