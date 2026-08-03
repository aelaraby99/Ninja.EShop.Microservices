using MassTransit;
using NINJA.EShop.Ordering.Application.Orders.EventHandlers.Integration;
using NINJA.EShop.Ordering.Application.Sagas.OrderProcessing;
using NINJA.EShop.Shared.Messaging.MassTransit;

namespace NINJA.EShop.Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderingDb");
            // Order matters: EF invokes SavingChanges(Async) interceptors in registration order, and
            // domain events (dispatched below) should see the audit fields already stamped on the
            // aggregate. Keep AuditableEntityInterceptor registered before DispatchDomainEventsInterceptor.
            // Stamps CreatedAt/CreatedBy/LastModified(By) on IEntity rows as they're saved
            services.AddScoped<ISaveChangesInterceptor,AuditableEntityInterceptor>();
            // Publishes an aggregate's queued domain events via MediatR during SaveChanges
            services.AddScoped<ISaveChangesInterceptor,DispatchDomainEventsInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp,options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()); // wires up both interceptors above
                options.UseSqlServer(connectionString);
            });
            // IApplicationDbContext must resolve to the SAME scoped instance as ApplicationDbContext:
            // the MassTransit outbox below (AddEntityFrameworkOutbox<ApplicationDbContext>) and the
            // interceptors above only see writes made through that one instance's SaveChanges.
            services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

            // RabbitMQ/MassTransit bus + consumers discovered from the Application assembly (e.g. BasketCheckoutEventHandler)
            services.AddMessageBroker(typeof(BasketCheckoutEventHandler).Assembly,bus =>
            {
                // Transactional outbox: publishes made inside a consumer/handler scope are
                // buffered on ApplicationDbContext and only sent once SaveChanges commits.
                bus.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                // Inbox pattern: every auto-configured receive endpoint dedupes consumed
                // messages by MessageId via InboxState, guarding against redelivery.
                bus.AddConfigureEndpointsCallback((context,_,cfg) =>
                {
                    cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
                });

                // Order-confirmation saga: persists OrderProcessingState in the same ApplicationDbContext
                // used everywhere else, so saga transitions commit atomically with the rest of a request.
                bus.AddSagaStateMachine<OrderProcessingStateMachine,OrderProcessingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ExistingDbContext<ApplicationDbContext>();
                        r.UseSqlServer();
                    });
            },
            (context,cfg) =>
            {
                // Exchange names/types shared with every other service touching these messages.
                MessageTopology.Configure(cfg);

                // Explicit queue for the BasketCheckoutEvent consumer (was auto-named "basket-checkout-event-handler").
                cfg.ReceiveEndpoint("ordering.basket-checkout",e =>
                {
                    e.ConfigureConsumer<BasketCheckoutEventHandler>(context);
                });

                // Explicit queue for the saga: bound to all three of its events (OrderCreated,
                // StockReserved, StockReservationFailed) on the exchanges configured above.
                cfg.ReceiveEndpoint("ordering.order-processing-saga",e =>
                {
                    e.ConfigureSaga<OrderProcessingState>(context);
                });
            });

            return services;
        }
    }
}