namespace Warehouse.Application.Queries.GetInventoryDashboard;

public record GetInventoryDashboardResponse(
    ProductSummary? Products,
    SupplierSummary? Suppliers,
    StockMovementSummary? RecentActivity,
    int? UnreadNotifications);

public record ProductSummary(
    int TotalProducts,
    int ArchivedProducts,
    int OutOfStockProducts,
    int ProductsExpiringSoon);

public record SupplierSummary(
    int TotalSuppliers,
    int ActiveSuppliers);

public record StockMovementSummary(
    int RecentMovementsCount,
    DateTime? LastMovementAt);
