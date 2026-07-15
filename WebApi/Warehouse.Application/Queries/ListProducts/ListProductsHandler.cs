namespace Warehouse.Application.Queries.ListProducts;

using MediatR;
using Warehouse.Domain.Repositories;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    public ListProductsHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<List<ProductResponse>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = (await _productRepository.GetAllAsync(cancellationToken)).AsEnumerable();

        if (request.OnlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        return products
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.SKU, p.Description, p.Price, p.QuantityInStock,
                p.SupplierName, p.SupplierId, p.ExpiryDate, p.IsArchived, p.CreatedAt, p.LastUpdatedAt))
            .ToList();
    }
}