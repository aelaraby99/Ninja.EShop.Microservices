using MassTransit;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Ordering.Application.Orders.EventHandlers.Domain;

public class OrderCreatedEventHandler
    (IPublishEndpoint publishEndpoint,ILogger<OrderCreatedEventHandler> logger): INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent,CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}",domainEvent.GetType().Name);

        var order = domainEvent.order;
        // Publish a flat, versionable integration event rather than the domain aggregate itself,
        // so other services don't need a reference to Ordering.Domain to consume this message.
        var integrationEvent = new OrderCreatedIntegrationEvent
        {
            OrderId = order.Id.Value,
            CustomerId = order.CustomerId.Value,
            OrderName = order.OrderName.Value,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            Items = order.OrderItems
                .Select(item => new OrderCreatedItem(item.ProductId.Value,item.Quantity,item.Price))
                .ToList()
        };
        await publishEndpoint.Publish(integrationEvent,cancellationToken);
    }
}