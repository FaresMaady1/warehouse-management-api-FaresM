namespace Warehouse.Application.Commands.UpdateProductQuantity;

using MediatR;
using Warehouse.Domain.Repositories;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse?>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductQuantityHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public async Task<UpdateProductQuantityResponse?> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return null;

        product.UpdateQuantity(request.QuantityInStock);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new UpdateProductQuantityResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt);
    }
}