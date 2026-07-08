namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Products;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<Product>>
{
    private readonly IProductRepository _productRepository;
    public SearchProductsHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<List<Product>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_productRepository.Search(request.Name, request.Supplier));
}
