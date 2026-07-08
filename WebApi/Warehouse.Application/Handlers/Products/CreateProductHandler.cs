namespace Warehouse.Application.Handlers.Products;

using MediatR;
using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductRepository _productRepository;
    public CreateProductHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (_productRepository.SkuExists(request.SKU))
            throw new DomainException($"A product with SKU '{request.SKU}' already exists.");

        var product = Product.Create(request.Name, request.SKU, request.Description,
            request.Price, request.QuantityInStock, request.SupplierName, request.ExpiryDate);

        _productRepository.Add(product);
        return Task.FromResult(product);
    }
}
