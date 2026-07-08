namespace Warehouse.Application.Commands.Products;

using MediatR;
using Warehouse.Domain.Products;

public record UploadProductImageCommand(string ProductId, string FileName, string FilePath, byte[] Content) : IRequest<Product?>;
