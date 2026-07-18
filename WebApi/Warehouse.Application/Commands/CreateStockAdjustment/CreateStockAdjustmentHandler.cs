namespace Warehouse.Application.Commands.CreateStockAdjustment;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.StockMovements;

public class CreateStockAdjustmentHandler : IRequestHandler<CreateStockAdjustmentCommand, CreateStockAdjustmentResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<CreateStockAdjustmentHandler> _logger;

    public CreateStockAdjustmentHandler(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        ICacheService cache,
        ILogger<CreateStockAdjustmentHandler> logger)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _cache = cache;
        _logger = logger;
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

        await _cache.RemoveAsync($"products:{product.Id}", cancellationToken);
        await _cache.RemoveAsync("products:list:True", cancellationToken);
        await _cache.RemoveAsync("products:list:False", cancellationToken);

        _logger.LogInformation(
            "Stock adjustment for product {ProductId}: {QuantityChanged} ({Reason}). New quantity: {NewQuantity}",
            product.Id, request.QuantityChanged, request.Reason, product.QuantityInStock);

        return new CreateStockAdjustmentResponse(
            movement.Id, product.Id, request.QuantityChanged, request.Reason,
            product.QuantityInStock, movement.OccurredAt);
    }
}