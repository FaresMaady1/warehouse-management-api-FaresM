namespace Warehouse.Application.Commands.DeleteSupplierDocument;

using MediatR;

public record DeleteSupplierDocumentCommand(string DocumentId) : IRequest<bool>;