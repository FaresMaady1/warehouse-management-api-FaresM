namespace Warehouse.Application.Queries.DownloadSupplierDocument;

public record DownloadSupplierDocumentResponse(string FileName, string ContentType, Stream Content);