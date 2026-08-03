namespace Warehouse.Application.Commands.UploadSupplierDocument;

using MediatR;
using Warehouse.Application.Events;
using Warehouse.Domain.Events;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.Storage;
using Warehouse.Domain.SupplierDocuments;

public class UploadSupplierDocumentHandler : IRequestHandler<UploadSupplierDocumentCommand, UploadSupplierDocumentResponse?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _documentRepository;
    private readonly IFileStorageService _storage;
    private readonly IEventPublisher _eventPublisher;

    public UploadSupplierDocumentHandler(
        ISupplierRepository supplierRepository,
        ISupplierDocumentRepository documentRepository,
        IFileStorageService storage,
        IEventPublisher eventPublisher)
    {
        _supplierRepository = supplierRepository;
        _documentRepository = documentRepository;
        _storage = storage;
        _eventPublisher = eventPublisher;
    }

    public async Task<UploadSupplierDocumentResponse?> Handle(UploadSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier == null) return null;

        var objectKey = $"suppliers/{request.SupplierId}/{Guid.NewGuid()}-{request.FileName}";
        await _storage.UploadAsync(objectKey, request.Content, request.ContentType, cancellationToken);

        var document = SupplierDocument.Create(request.SupplierId, request.FileName, objectKey, request.ContentType, request.SizeBytes);
        _documentRepository.Add(document);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync("file.uploaded", new WarehouseFileUploadedEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid().ToString(),
            request.SupplierId,
            "Supplier",
            request.FileName,
            request.ContentType,
            request.SizeBytes), cancellationToken);

        return new UploadSupplierDocumentResponse(document.Id, document.SupplierId, document.FileName, document.UploadedAt);
    }
}
