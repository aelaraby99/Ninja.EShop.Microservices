namespace NINJA.EShop.Ordering.Domain.Events;

public record OrderUpdatedEvent(Order order): IDomainEvent;