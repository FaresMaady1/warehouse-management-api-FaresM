namespace Warehouse.Application.Commands.UploadSupplierDocument;

using MediatR;

public record UploadSupplierDocumentCommand(string SupplierId, string FileName, string ContentType, long SizeBytes, Stream Content) : IRequest<UploadSupplierDocumentResponse?>;