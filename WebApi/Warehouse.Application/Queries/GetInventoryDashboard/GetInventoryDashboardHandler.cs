namespace Warehouse.Application.Queries.GetInventoryDashboard;

using MediatR;
using Warehouse.Domain.Repositories;

public class GetInventoryDashboardHandler : IRequestHandler<GetInventoryDashboardQuery, GetInventoryDashboardResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public GetInventoryDashboardHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<GetInventoryDashboardResponse> Handle(GetInventoryDashboardQuery request, CancellationToken cancellationToken)
    {
        var productsTask = _productRepository.GetAllAsync(cancellationToken);
        var suppliersTask = _supplierRepository.GetAllAsync(cancellationToken);

        await Task.WhenAll(productsTask, suppliersTask);

        var products = await productsTask;
        var suppliers = await suppliersTask;

        var expiringSoonCutoff = DateTime.Now.AddDays(30);

        return new GetInventoryDashboardResponse(
            TotalProducts: products.Count,
            ArchivedProducts: products.Count(p => p.IsArchived),
            OutOfStockProducts: products.Count(p => p.QuantityInStock == 0 && !p.IsArchived),
            ProductsExpiringSoon: products.Count(p => !p.IsArchived && p.ExpiryDate <= expiringSoonCutoff),
            TotalSuppliers: suppliers.Count,
            ActiveSuppliers: suppliers.Count(s => s.IsActive));
    }
}