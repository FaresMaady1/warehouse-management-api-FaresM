namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Products;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, Product?>
{
    private readonly IProductRepository _productRepository;
    public UpdateProductQuantityHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product?> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        if (product == null) return Task.FromResult<Product?>(null);

        product.UpdateQuantity(request.QuantityInStock);
        return Task.FromResult<Product?>(product);
    }
}
