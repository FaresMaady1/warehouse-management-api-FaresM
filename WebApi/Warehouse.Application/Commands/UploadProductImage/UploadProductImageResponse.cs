namespace Warehouse.Application.Commands.UploadProductImage;

// Matches the original controller's `Ok(new { fileName, filePath })` response shape.
public record UploadProductImageResponse(string FileName, string FilePath);
