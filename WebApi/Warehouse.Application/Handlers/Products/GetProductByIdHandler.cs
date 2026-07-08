namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Products;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IProductRepository _productRepository;
    public GetProductByIdHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_productRepository.GetById(request.Id));
}
