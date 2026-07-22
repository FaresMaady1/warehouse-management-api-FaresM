namespace Warehouse.Application.Commands.UploadSupplierDocument;

public record UploadSupplierDocumentResponse(string Id, string SupplierId, string FileName, DateTime UploadedAt);