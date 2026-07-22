namespace Warehouse.Domain.ProductImages;

public class ProductImage
{
    public string ProductId { get; private set; } = default!;
    public string FileName { get; private set; } = default!;
    public string ObjectKey { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;

    private ProductImage() { }

    public static ProductImage Create(string productId, string fileName, string objectKey, string contentType)
        => new() { ProductId = productId, FileName = fileName, ObjectKey = objectKey, ContentType = contentType };
}