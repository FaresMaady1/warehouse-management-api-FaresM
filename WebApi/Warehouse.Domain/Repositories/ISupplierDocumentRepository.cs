namespace Warehouse.Domain.Repositories;

using Warehouse.Domain.SupplierDocuments;

public interface ISupplierDocumentRepository
{
    Task<SupplierDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    void Add(SupplierDocument document);
    void Remove(SupplierDocument document);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}