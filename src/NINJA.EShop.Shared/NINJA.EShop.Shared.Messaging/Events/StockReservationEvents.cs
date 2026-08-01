namespace NINJA.EShop.Shared.Messaging.Events;

public record ReserveStock(Guid OrderId, List<OrderCreatedIntegrationEventItem> Items) : IntegrationEvent;

public record StockReserved(Guid OrderId) : IntegrationEvent;

public record StockReservationFailed(Guid OrderId, string Reason) : IntegrationEvent;
