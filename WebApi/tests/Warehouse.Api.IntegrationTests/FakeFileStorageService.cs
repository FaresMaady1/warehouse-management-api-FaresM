namespace Warehouse.Api.IntegrationTests;

using System.Collections.Concurrent;
using Warehouse.Domain.Storage;

public class FakeFileStorageService : IFileStorageService
{
    private readonly ConcurrentDictionary<string, byte[]> _store = new();

    // UploadAsync
    public async Task UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        _store[objectKey] = buffer.ToArray();
    }

    // DownloadAsync
    public Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(new MemoryStream(_store[objectKey]));

    // DeleteAsync
    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        _store.TryRemove(objectKey, out _);
        return Task.CompletedTask;
    }
}