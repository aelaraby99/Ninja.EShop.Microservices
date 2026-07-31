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
        var items = order.OrderItems
            .Select(item => new OrderCreatedIntegrationEventItem(item.ProductId.Value,item.Quantity))
            .ToList();

        // Published through the EF Core transactional outbox: buffered on this scope's
        // DbContext and only sent once the Order row is committed in the same SaveChanges call.
        await publishEndpoint.Publish(
            new OrderCreatedIntegrationEvent(order.Id.Value,order.CustomerId.Value,items),
            cancellationToken);
    }
}