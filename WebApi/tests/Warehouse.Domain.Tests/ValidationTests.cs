namespace Warehouse.Domain.Tests;

using System.ComponentModel.DataAnnotations;
using WebApi.Contracts;
using Xunit;

public class ValidationTests
{
    [Fact]
    public void CreateProductRequest_Fails_When_Name_Is_Empty()
    {
        var request = new CreateProductRequest
        {
            Name = "", SKU = "SKU-1", Description = "desc", Price = 10,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddYears(1)
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void CreateProductRequest_Fails_When_ExpiryDate_Is_In_The_Past()
    {
        var request = new CreateProductRequest
        {
            Name = "Mouse", SKU = "SKU-1", Description = "desc", Price = 10,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddDays(-1)
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.False(isValid);
    }

    [Fact]
    public void CreateProductRequest_Passes_With_Valid_Data()
    {
        var request = new CreateProductRequest
        {
            Name = "Mouse", SKU = "SKU-1", Description = "desc", Price = 10,
            QuantityInStock = 5, SupplierName = "Acme", ExpiryDate = DateTime.Now.AddYears(1)
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.True(isValid);
    }
}