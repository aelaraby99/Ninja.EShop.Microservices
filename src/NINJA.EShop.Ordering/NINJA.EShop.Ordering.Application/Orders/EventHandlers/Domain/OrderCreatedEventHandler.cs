using MassTransit;

namespace NINJA.EShop.Ordering.Application.Orders.EventHandlers.Domain;

public class OrderCreatedEventHandler
    (IPublishEndpoint publishEndpoint,ILogger<OrderCreatedEventHandler> logger): INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent,CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}",domainEvent.GetType().Name);

        var order = domainEvent.order;
        await publishEndpoint.Publish(new OrderCreatedEvent(order));
    }
}