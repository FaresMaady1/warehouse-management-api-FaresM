namespace WebApi.ViewModels;

public class SupplierViewModel
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Country { get; set; } = default!;
    public bool IsActive { get; set; }
}