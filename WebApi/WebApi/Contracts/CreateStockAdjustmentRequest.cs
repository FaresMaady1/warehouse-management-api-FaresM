namespace WebApi.Contracts;

using System.ComponentModel.DataAnnotations;

public class CreateStockAdjustmentRequest : IValidatableObject
{
    [Required]
    public string ProductId { get; set; } = default!;

    public int QuantityChanged { get; set; }

    [StringLength(200)]
    public string? Reason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (QuantityChanged == 0)
            yield return new ValidationResult("QuantityChanged cannot be zero.", new[] { nameof(QuantityChanged) });

        if (QuantityChanged < 0 && string.IsNullOrWhiteSpace(Reason))
            yield return new ValidationResult("Reason is required when decreasing stock.", new[] { nameof(Reason) });
    }
}