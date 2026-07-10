namespace Warehouse.Application.Commands.ArchiveProduct;

using MediatR;

public record ArchiveProductCommand(string ProductId) : IRequest<ArchiveProductResponse?>;
