namespace Warehouse.Application.Commands.UploadProductImage;

using MediatR;

public record UploadProductImageCommand(string ProductId, string FileName, string FilePath, byte[] Content) : IRequest<UploadProductImageResponse?>;
