namespace Warehouse.Domain.StockMovements;

public class StockMovement
{
    public string Id { get; private set; } = default!;
    public string ProductId { get; private set; } = default!;
    public int QuantityChanged { get; private set; }
    public string Reason { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(string productId, int quantityChanged, string reason)
        => new()
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = productId,
            QuantityChanged = quantityChanged,
            Reason = reason,
            OccurredAt = DateTime.Now
        };
}
