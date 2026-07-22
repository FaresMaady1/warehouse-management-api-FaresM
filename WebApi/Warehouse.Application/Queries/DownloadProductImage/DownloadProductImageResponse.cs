namespace Warehouse.Application.Queries.DownloadProductImage;

public record DownloadProductImageResponse(string FileName, string ContentType, Stream Content);