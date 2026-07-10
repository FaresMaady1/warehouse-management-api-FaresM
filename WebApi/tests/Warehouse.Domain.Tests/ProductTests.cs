namespace Warehouse.Domain.Tests;

using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Products;
using Warehouse.Domain.Suppliers;
using Xunit;

public class ProductTests
{
    [Fact]
    public void Create_Throws_When_Price_Is_Zero_Or_Negative()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("Mouse", "SKU-1", "desc", 0m, 10, "Supplier", DateTime.Now.AddYears(1)));
    }

    [Fact]
    public void Create_Throws_When_Quantity_Is_Negative()
    {
        Assert.Throws<DomainException>(() =>
            Product.Create("Mouse", "SKU-1", "desc", 10m, -1, "Supplier", DateTime.Now.AddYears(1)));
    }

    [Fact]
    public void UpdatePrice_Throws_When_Product_Is_Archived()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Supplier", DateTime.Now.AddYears(1));
        product.Archive();

        Assert.Throws<DomainException>(() => product.UpdatePrice(20m));
    }

    [Fact]
    public void AssignSupplier_Throws_When_Supplier_Is_Inactive()
    {
        var product = Product.Create("Mouse", "SKU-1", "desc", 10m, 5, "Supplier", DateTime.Now.AddYears(1));
        var supplier = Supplier.Create("Acme", "USA", "a@acme.com", "123");
        supplier.Deactivate();

        Assert.Throws<DomainException>(() => product.AssignSupplier(supplier));
    }
}
