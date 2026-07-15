namespace WebApi.Contracts;

using System.ComponentModel.DataAnnotations;

public class CreateSupplierRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Country { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = default!;

    [Required]
    public string PhoneNumber { get; set; } = default!;
}