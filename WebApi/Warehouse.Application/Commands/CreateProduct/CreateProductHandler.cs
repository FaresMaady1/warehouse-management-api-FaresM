namespace Warehouse.Application.Commands.CreateProduct;

using MediatR;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;
using Warehouse.Domain.Repositories;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;
    public CreateProductHandler(IProductRepository productRepository) => _productRepository = productRepository;

    public Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (_productRepository.SkuExists(request.SKU))
            throw new DomainException($"A product with SKU '{request.SKU}' already exists.");

        var product = Product.Create(request.Name, request.SKU, request.Description,
            request.Price, request.QuantityInStock, request.SupplierName, request.ExpiryDate);

        _productRepository.Add(product);
        _productRepository.SaveChanges();

        return Task.FromResult(new CreateProductResponse(
            product.Id, product.Name, product.SKU, product.Description, product.Price,
            product.QuantityInStock, product.SupplierName, product.SupplierId, product.ExpiryDate,
            product.IsArchived, product.CreatedAt, product.LastUpdatedAt));
    }
}
