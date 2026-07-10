namespace Warehouse.Domain.WarehouseItems;

public class WarehouseItem
{
    public string Id { get; private set; } = default!;
    public string ProductId { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public int Quantity { get; private set; }

    private WarehouseItem() { }

    public static WarehouseItem Create(string productId, string location, int quantity)
        => new()
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = productId,
            Location = location,
            Quantity = quantity
        };
}
