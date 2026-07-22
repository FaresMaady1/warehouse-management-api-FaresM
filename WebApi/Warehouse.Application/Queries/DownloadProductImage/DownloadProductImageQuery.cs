namespace Warehouse.Application.Queries.DownloadProductImage;

using MediatR;

public record DownloadProductImageQuery(string ProductId) : IRequest<DownloadProductImageResponse?>;