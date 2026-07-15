namespace WebApi.Contracts;

using System.ComponentModel.DataAnnotations;

public class CreateProductRequest : IValidatableObject
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string SKU { get; set; } = default!;
    
    public string Description { get; set; } = default!;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    [Required]
    public string SupplierName { get; set; } = default!;

    public DateTime ExpiryDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiryDate <= DateTime.Now)
            yield return new ValidationResult("ExpiryDate must be in the future.", new[] { nameof(ExpiryDate) });
    }
}