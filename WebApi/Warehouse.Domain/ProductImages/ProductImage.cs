namespace Warehouse.Domain.ProductImages;

public class ProductImage
{
    public string ProductId { get; private set; } = default!;
    public string FileName { get; private set; } = default!;
    public string FilePath { get; private set; } = default!;

    private ProductImage() { }

    public static ProductImage Create(string productId, string fileName, string filePath)
        => new() { ProductId = productId, FileName = fileName, FilePath = filePath };
}
