namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Products;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<Product>>
{
    private readonly IProductRepository _productRepository;
    public ListProductsHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<List<Product>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _productRepository.GetAll().AsEnumerable();

        if (request.OnlyAvailable)
            products = products.Where(p => p.QuantityInStock > 0 && !p.IsArchived);

        return Task.FromResult(products.OrderByDescending(p => p.CreatedAt).ToList());
    }
}
