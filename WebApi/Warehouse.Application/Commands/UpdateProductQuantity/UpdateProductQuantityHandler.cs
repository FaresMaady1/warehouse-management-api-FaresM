namespace Warehouse.Application.Commands.UpdateProductQuantity;

using MediatR;
using Warehouse.Domain.Repositories;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse?>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductQuantityHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<UpdateProductQuantityResponse?> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<UpdateProductQuantityResponse?>(null);

        product.UpdateQuantity(request.QuantityInStock);

        return Task.FromResult<UpdateProductQuantityResponse?>(new UpdateProductQuantityResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
