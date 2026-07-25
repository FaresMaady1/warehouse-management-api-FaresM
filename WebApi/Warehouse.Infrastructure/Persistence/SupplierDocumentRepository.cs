namespace Warehouse.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.SupplierDocuments;

public class SupplierDocumentRepository : ISupplierDocumentRepository
{
    private readonly WarehouseDbContext _context;
    public SupplierDocumentRepository(WarehouseDbContext context) => _context = context;

    public Task<SupplierDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        _context.SupplierDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(SupplierDocument document) => _context.SupplierDocuments.Add(document);

    public void Remove(SupplierDocument document) => _context.SupplierDocuments.Remove(document);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}