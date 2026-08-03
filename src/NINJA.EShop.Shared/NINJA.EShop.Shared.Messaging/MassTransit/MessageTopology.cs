using MassTransit;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Shared.Messaging.MassTransit;

// Single source of truth for exchange names/types across services. Every service that publishes
// or consumes a given message type must call Configure() so its exchange name/type agree with
// every other service touching that same message - a mismatch here silently misroutes messages
// with no error, rather than raising a visible failure.
public static class MessageTopology
{
    public static void Configure(IRabbitMqBusFactoryConfigurator cfg)
    {
        // Point-to-point: exactly one known consumer today.
        cfg.Message<BasketCheckoutEvent>(m => m.SetEntityName("basket.basket-checkout"));
        cfg.Publish<BasketCheckoutEvent>(p => p.ExchangeType = "direct");

        // Broadcast: multiple consumers are plausible (the saga today, a future Notification service, etc.).
        cfg.Message<OrderCreatedIntegrationEvent>(m => m.SetEntityName("ordering.order-created"));
        cfg.Publish<OrderCreatedIntegrationEvent>(p => p.ExchangeType = "fanout");

        // Point-to-point: saga -> Catalog's stock reservation, single known consumer.
        cfg.Message<ReserveStock>(m => m.SetEntityName("ordering.reserve-stock"));
        cfg.Publish<ReserveStock>(p => p.ExchangeType = "direct");

        // Point-to-point replies: Catalog -> the saga, single known consumer.
        cfg.Message<StockReserved>(m => m.SetEntityName("catalog.stock-reserved"));
        cfg.Publish<StockReserved>(p => p.ExchangeType = "direct");

        cfg.Message<StockReservationFailed>(m => m.SetEntityName("catalog.stock-reservation-failed"));
        cfg.Publish<StockReservationFailed>(p => p.ExchangeType = "direct");
    }
}
