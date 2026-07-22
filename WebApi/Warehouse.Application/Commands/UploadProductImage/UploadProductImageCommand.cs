namespace Warehouse.Application.Commands.UploadProductImage;

using MediatR;

public record UploadProductImageCommand(string ProductId, string FileName, string ContentType, Stream Content) : IRequest<UploadProductImageResponse?>;