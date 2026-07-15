namespace Warehouse.Application.Commands.UpdateProductPrice;

using MediatR;
using Warehouse.Domain.Repositories;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResponse?>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductPriceHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<UpdateProductPriceResponse?> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<UpdateProductPriceResponse?>(null);

        product.UpdatePrice(request.Price);
        _productRepository.SaveChanges();

        return Task.FromResult<UpdateProductPriceResponse?>(new UpdateProductPriceResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
