namespace NINJA.EShop.Ordering.Infrastructure.Data.Interceptors;

public class DispatchDomainEventsInterceptor(IMediator mediator): SaveChangesInterceptor
{
    // This app only ever saves via SaveChangesAsync. The sync path would have to block on the
    // async mediator.Publish chain below (sync-over-async), risking thread-pool starvation or
    // deadlocks under load, so it fails fast instead of doing that silently.
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,InterceptionResult<int> result)
    {
        throw new NotSupportedException($"{nameof(ApplicationDbContext)} must be saved via SaveChangesAsync, not the synchronous SaveChanges, so domain events can be dispatched asynchronously.");
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,InterceptionResult<int> result,CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context,cancellationToken);
        return await base.SavingChangesAsync(eventData,result,cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context,CancellationToken cancellationToken)
    {
        if (context == null)
            return;

        // Dispatch in passes rather than a single snapshot: a handler can cause another tracked
        // aggregate to queue a new domain event as a side effect, and that event needs a further
        // pass to be picked up. maxPasses guards against a handler that re-queues events forever.
        const int maxPasses = 10;
        for (var pass = 0; pass < maxPasses; pass++)
        {
            var aggregatesWithEvents = context.ChangeTracker
                .Entries<IAggregate>()
                .Where(a => a.Entity.DomainEvents.Any())
                .Select(a => a.Entity)
                .ToList();

            if (aggregatesWithEvents.Count == 0)
                return;

            var domainEvents = aggregatesWithEvents
                .SelectMany(a => a.ClearDomainEvents())
                .ToList();

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent,cancellationToken);
        }

        throw new InvalidOperationException($"Domain event dispatch did not settle after {maxPasses} passes; a handler is likely re-queuing events indefinitely.");
    }
}