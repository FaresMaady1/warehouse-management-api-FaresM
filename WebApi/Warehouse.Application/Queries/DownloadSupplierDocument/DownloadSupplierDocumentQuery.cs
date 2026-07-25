namespace Warehouse.Application.Queries.DownloadSupplierDocument;

using MediatR;

public record DownloadSupplierDocumentQuery(string DocumentId) : IRequest<DownloadSupplierDocumentResponse?>;