namespace Warehouse.Domain.Suppliers;

using Warehouse.Domain.Exceptions;

public class Supplier
{
    public string Id { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string ContactEmail { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Supplier() { }

    public static Supplier Create(string name, string country, string contactEmail, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Supplier name is required.");

        return new Supplier
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Country = country,
            ContactEmail = contactEmail,
            PhoneNumber = phoneNumber,
            IsActive = true
        };
    }

    // Used only by Infrastructure to rehydrate seed/stored data with a fixed Id.
    public static Supplier Restore(string id, string name, string country, string contactEmail, string phoneNumber, bool isActive)
    {
        return new Supplier
        {
            Id = id,
            Name = name,
            Country = country,
            ContactEmail = contactEmail,
            PhoneNumber = phoneNumber,
            IsActive = isActive
        };
    }

    public void Deactivate() => IsActive = false;
}
