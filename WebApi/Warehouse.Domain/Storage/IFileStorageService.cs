namespace Warehouse.Domain.Storage;

public interface IFileStorageService
{
    Task UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
}