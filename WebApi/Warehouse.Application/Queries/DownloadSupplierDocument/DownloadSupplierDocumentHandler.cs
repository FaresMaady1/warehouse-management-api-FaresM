namespace Warehouse.Application.Queries.DownloadSupplierDocument;

using MediatR;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;

public class DownloadSupplierDocumentHandler : IRequestHandler<DownloadSupplierDocumentQuery, DownloadSupplierDocumentResponse?>
{
    private readonly ISupplierDocumentRepository _documentRepository;
    private readonly IFileStorageService _storage;

    public DownloadSupplierDocumentHandler(ISupplierDocumentRepository documentRepository, IFileStorageService storage)
    {
        _documentRepository = documentRepository;
        _storage = storage;
    }

    public async Task<DownloadSupplierDocumentResponse?> Handle(DownloadSupplierDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document == null) return null;

        var stream = await _storage.DownloadAsync(document.ObjectKey, cancellationToken);
        return new DownloadSupplierDocumentResponse(document.FileName, document.ContentType, stream);
    }
}