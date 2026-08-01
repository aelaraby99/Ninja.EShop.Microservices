namespace NINJA.EShop.Shared.Messaging.Events;

public record OrderCreatedIntegrationEvent(Guid OrderId, Guid CustomerId, List<OrderCreatedIntegrationEventItem> Items) : IntegrationEvent;

public record OrderCreatedIntegrationEventItem(Guid ProductId, int Quantity);
