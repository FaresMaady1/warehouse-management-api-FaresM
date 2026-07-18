namespace Warehouse.Application.Queries.GetInventoryDashboard;

using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Repositories;

public class GetInventoryDashboardHandler : IRequestHandler<GetInventoryDashboardQuery, GetInventoryDashboardResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly ILogger<GetInventoryDashboardHandler> _logger;

    public GetInventoryDashboardHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IStockMovementRepository stockMovementRepository,
        ILogger<GetInventoryDashboardHandler> logger)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _stockMovementRepository = stockMovementRepository;
        _logger = logger;
    }

    public async Task<GetInventoryDashboardResponse> Handle(GetInventoryDashboardQuery request, CancellationToken cancellationToken)
    {
        var productsTask = GetProductSummaryAsync(cancellationToken);
        var suppliersTask = GetSupplierSummaryAsync(cancellationToken);
        var recentActivityTask = GetStockMovementSummaryAsync(cancellationToken);

        var products = await productsTask;
        var suppliers = await suppliersTask;
        var recentActivity = await recentActivityTask;

        return new GetInventoryDashboardResponse(products, suppliers, recentActivity);
    }

    private async Task<ProductSummary?> GetProductSummaryAsync(CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            var expiringSoonCutoff = DateTime.Now.AddDays(30);

            return new ProductSummary(
                TotalProducts: products.Count,
                ArchivedProducts: products.Count(p => p.IsArchived),
                OutOfStockProducts: products.Count(p => p.QuantityInStock == 0 && !p.IsArchived),
                ProductsExpiringSoon: products.Count(p => !p.IsArchived && p.ExpiryDate <= expiringSoonCutoff));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load product summary for the inventory dashboard.");
            return null;
        }
    }

    private async Task<SupplierSummary?> GetSupplierSummaryAsync(CancellationToken cancellationToken)
    {
        try
        {
            var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);

            return new SupplierSummary(
                TotalSuppliers: suppliers.Count,
                ActiveSuppliers: suppliers.Count(s => s.IsActive));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load supplier summary for the inventory dashboard.");
            return null;
        }
    }

    private async Task<StockMovementSummary?> GetStockMovementSummaryAsync(CancellationToken cancellationToken)
    {
        try
        {
            var recentMovements = await _stockMovementRepository.GetRecentAsync(10, cancellationToken);

            return new StockMovementSummary(
                RecentMovementsCount: recentMovements.Count,
                LastMovementAt: recentMovements.Count > 0 ? recentMovements[0].OccurredAt : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recent stock activity for the inventory dashboard.");
            return null;
        }
    }
}