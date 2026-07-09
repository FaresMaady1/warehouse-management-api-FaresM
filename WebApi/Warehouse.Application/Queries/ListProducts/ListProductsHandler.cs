namespace Warehouse.Application.Queries.ListProducts;

using MediatR;
using Warehouse.Domain.Repositories;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    public ListProductsHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<List<ProductResponse>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _productRepository.GetAll().AsEnumerable();

        if (request.OnlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        var response = products
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductResponse(
                p.Id, p.Name, p.SKU, p.Description, p.Price, p.QuantityInStock,
                p.SupplierName, p.SupplierId, p.ExpiryDate, p.IsArchived, p.CreatedAt, p.LastUpdatedAt))
            .ToList();

        return Task.FromResult(response);
    }
}
