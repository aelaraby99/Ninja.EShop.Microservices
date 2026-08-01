using MassTransit;
using Microsoft.FeatureManagement;

namespace NINJA.EShop.Ordering.Application.Orders.EventHandlers.Domain;

public class OrderCreatedEventHandler
    (IPublishEndpoint publishEndpoint,ILogger<OrderCreatedEventHandler> logger,IFeatureManager featureManager): INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent,CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}",domainEvent.GetType().Name);

        if (await featureManager.IsEnabledAsync("OrderFullfilment"))
        {
            var orderCreatedIntegrationEvent = domainEvent.order.ToOrderDto();
            await publishEndpoint.Publish(orderCreatedIntegrationEvent,cancellationToken);
        }
    }
}