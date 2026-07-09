namespace Warehouse.Application.Queries.GetProductById;

using MediatR;
using Warehouse.Domain.Repositories;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse?>
{
    private readonly IProductRepository _productRepository;
    public GetProductByIdHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<GetProductByIdResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.Id);
        if (product == null) return Task.FromResult<GetProductByIdResponse?>(null);

        return Task.FromResult<GetProductByIdResponse?>(new GetProductByIdResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
