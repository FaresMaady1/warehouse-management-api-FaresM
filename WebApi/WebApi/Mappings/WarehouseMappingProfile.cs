namespace WebApi.Mappings;

using AutoMapper;
using WebApi.ViewModels;
using Warehouse.Application.Commands.CreateProduct;
using Warehouse.Application.Commands.UpdateProductPrice;
using Warehouse.Application.Commands.UpdateProductQuantity;
using Warehouse.Application.Commands.ArchiveProduct;
using Warehouse.Application.Commands.AssignSupplierToProduct;
using Warehouse.Application.Commands.CreateSupplier;
using Warehouse.Application.Commands.DeactivateSupplier;
using Warehouse.Application.Queries.GetProductById;
using Warehouse.Application.Queries.GetSupplierById;
using Warehouse.Application.Queries.ListSuppliers;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<CreateProductResponse, ProductViewModel>();
        CreateMap<UpdateProductPriceResponse, ProductViewModel>();
        CreateMap<UpdateProductQuantityResponse, ProductViewModel>();
        CreateMap<ArchiveProductResponse, ProductViewModel>();
        CreateMap<AssignSupplierToProductResponse, ProductViewModel>();
        CreateMap<GetProductByIdResponse, ProductViewModel>();
        CreateMap<Warehouse.Application.Queries.ListProducts.ProductResponse, ProductViewModel>();
        CreateMap<Warehouse.Application.Queries.SearchProducts.ProductResponse, ProductViewModel>();

        CreateMap<CreateSupplierResponse, SupplierViewModel>();
        CreateMap<DeactivateSupplierResponse, SupplierViewModel>();
        CreateMap<GetSupplierByIdResponse, SupplierViewModel>();
        CreateMap<Warehouse.Application.Queries.ListSuppliers.SupplierResponse, SupplierViewModel>();
    }
}