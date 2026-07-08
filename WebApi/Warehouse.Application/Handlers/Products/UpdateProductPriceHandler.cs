namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Products;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand, Product?>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductPriceHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product?> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<Product?>(null);

        product.UpdatePrice(request.Price);
        return Task.FromResult<Product?>(product);
    }
}
