namespace Warehouse.Application.Commands.CreateStockAdjustment;

using MediatR;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.StockMovements;

public class CreateStockAdjustmentHandler : IRequestHandler<CreateStockAdjustmentCommand, CreateStockAdjustmentResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;

    public CreateStockAdjustmentHandler(IProductRepository productRepository, IStockMovementRepository stockMovementRepository)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<CreateStockAdjustmentResponse> Handle(CreateStockAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new NotFoundException($"Product '{request.ProductId}' was not found.");

        product.UpdateQuantity(product.QuantityInStock + request.QuantityChanged);

        var movement = StockMovement.Create(product.Id, request.QuantityChanged, request.Reason);
        _stockMovementRepository.Add(movement);

        await _productRepository.SaveChangesAsync(cancellationToken);
        await _stockMovementRepository.SaveChangesAsync(cancellationToken);

        return new CreateStockAdjustmentResponse(
            movement.Id, product.Id, request.QuantityChanged, request.Reason,
            product.QuantityInStock, movement.OccurredAt);
    }
}