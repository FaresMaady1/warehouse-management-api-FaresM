namespace Warehouse.Application.Commands.DeleteSupplierDocument;

using MediatR;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;

public class DeleteSupplierDocumentHandler : IRequestHandler<DeleteSupplierDocumentCommand, bool>
{
    private readonly ISupplierDocumentRepository _documentRepository;
    private readonly IFileStorageService _storage;

    public DeleteSupplierDocumentHandler(ISupplierDocumentRepository documentRepository, IFileStorageService storage)
    {
        _documentRepository = documentRepository;
        _storage = storage;
    }

    public async Task<bool> Handle(DeleteSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document == null) return false;

        await _storage.DeleteAsync(document.ObjectKey, cancellationToken);
        _documentRepository.Remove(document);
        await _documentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}