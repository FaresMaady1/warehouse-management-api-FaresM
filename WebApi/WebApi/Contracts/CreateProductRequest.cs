namespace WebApi.Contracts;

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

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
        {
            var localizer = validationContext.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            var message = localizer?["ExpiryDateMustBeFuture"].Value ?? "ExpiryDate must be in the future.";
            yield return new ValidationResult(message, new[] { nameof(ExpiryDate) });
        }
    }
}