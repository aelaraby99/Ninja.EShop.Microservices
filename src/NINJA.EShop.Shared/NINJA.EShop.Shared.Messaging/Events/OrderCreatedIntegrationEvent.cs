namespace NINJA.EShop.Shared.Messaging.Events;

public record OrderCreatedIntegrationEvent: IntegrationEvent
{
    public Guid OrderId { get; set; } = default!;
    public Guid CustomerId { get; set; } = default!;
    public string OrderName { get; set; } = default!;
    public decimal TotalPrice { get; set; } = default!;
    public string Status { get; set; } = default!;
    public List<OrderCreatedItem> Items { get; set; } = [];
}
public record OrderCreatedItem(Guid ProductId,int Quantity,decimal Price);
