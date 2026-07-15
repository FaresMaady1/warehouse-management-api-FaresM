namespace Warehouse.Application.Queries.GetInventoryDashboard;

public record GetInventoryDashboardResponse(
    int TotalProducts,
    int ArchivedProducts,
    int OutOfStockProducts,
    int ProductsExpiringSoon,
    int TotalSuppliers,
    int ActiveSuppliers);