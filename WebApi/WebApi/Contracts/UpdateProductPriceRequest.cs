namespace WebApi.Contracts;

using System.ComponentModel.DataAnnotations;

public class UpdateProductPriceRequest
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
}