namespace Warehouse.Application.Caching;

public static class CacheKeys
{
    public static string Product(string productId) => $"products:{productId}";
    public static string ProductList(bool onlyAvailable) => $"products:list:{onlyAvailable}";
    public const string SuppliersAll = "suppliers:all";
    public static string Supplier(string supplierId) => $"suppliers:{supplierId}";
}