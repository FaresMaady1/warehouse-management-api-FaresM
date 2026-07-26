namespace Warehouse.Application.Commands.CreateStockAdjustment;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Caching;
using Warehouse.Application.Events;
using Warehouse.Domain.Caching;
using Warehouse.Domain.Events;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Repositories;
using Warehouse.Domain.StockMovements;

public class CreateStockAdjustmentHandler : IRequestHandler<CreateStockAdjustmentCommand, CreateStockAdjustmentResponse>
{
    // Keeping this as a plain constant for now instead of a per-product setting - can move it to
    // appsettings later if we ever need different thresholds per category.
    private const int LowStockThreshold = 10;

    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly ICacheService _cache;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateStockAdjustmentHandler> _logger;

    public CreateStockAdjustmentHandler(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        ICacheService cache,
        IEventPublisher eventPublisher,
        ILogger<CreateStockAdjustmentHandler> logger)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _cache = cache;
        _eventPublisher = eventPublisher;
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

        await _cache.RemoveAsync(CacheKeys.Product(product.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(true), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.ProductList(false), cancellationToken);

        _logger.LogInformation(
            "Stock adjustment for product {ProductId}: {QuantityChanged} ({Reason}). New quantity: {NewQuantity}",
            product.Id, request.QuantityChanged, request.Reason, product.QuantityInStock);

        if (product.QuantityInStock < LowStockThreshold)
        {
            // Fire-and-forget from the handler's point of view - a RabbitMQ hiccup shouldn't
            // fail a stock adjustment that already succeeded. See RabbitMqEventPublisher.
            await _eventPublisher.PublishAsync("stock.low", new StockLowDetectedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                product.Id,
                product.Name,
                product.SKU,
                product.QuantityInStock,
                LowStockThreshold), cancellationToken);
        }

        return new CreateStockAdjustmentResponse(
            movement.Id, product.Id, request.QuantityChanged, request.Reason,
            product.QuantityInStock, movement.OccurredAt);
    }
}
