namespace NINJA.EShop.Ordering.Application.Orders.EventHandlers.Domain;

public class OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger): INotificationHandler<OrderUpdatedEvent>
{
    public async Task Handle(OrderUpdatedEvent domainEvent,CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}",domainEvent.GetType().Name);
        throw new NotImplementedException();
    }
}