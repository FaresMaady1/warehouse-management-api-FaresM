namespace Warehouse.Domain.SupplierDocuments;

public class SupplierDocument
{
    public string Id { get; private set; } = default!;
    public string SupplierId { get; private set; } = default!;
    public string FileName { get; private set; } = default!;
    public string ObjectKey { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private SupplierDocument() { }

    public static SupplierDocument Create(string supplierId, string fileName, string objectKey, string contentType, long sizeBytes)
        => new()
        {
            Id = Guid.NewGuid().ToString(),
            SupplierId = supplierId,
            FileName = fileName,
            ObjectKey = objectKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            UploadedAt = DateTime.Now
        };
}