namespace Warehouse.Application.Queries.SearchProducts;

using MediatR;
using Warehouse.Domain.Repositories;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    public SearchProductsHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<List<ProductResponse>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchAsync(request.Name, request.Supplier, cancellationToken);

        return products
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.SKU, p.Description, p.Price, p.QuantityInStock,
                p.SupplierName, p.SupplierId, p.ExpiryDate, p.IsArchived, p.CreatedAt, p.LastUpdatedAt))
            .ToList();
    }
}