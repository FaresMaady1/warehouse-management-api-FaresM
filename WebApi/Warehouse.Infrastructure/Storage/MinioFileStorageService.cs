namespace Warehouse.Infrastructure.Storage;

using Minio;
using Minio.DataModel.Args;
using Warehouse.Domain.Storage;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioFileStorageService(IMinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }

    public Task UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default) =>
        _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType), cancellationToken);

    public async Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var buffer = new MemoryStream();
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithCallbackStream(stream => stream.CopyTo(buffer)), cancellationToken);

        buffer.Position = 0;
        return buffer;
    }

    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default) =>
        _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey), cancellationToken);
}