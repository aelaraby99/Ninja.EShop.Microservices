using MassTransit;
using NINJA.EShop.Ordering.Application.Orders.EventHandlers.Integration;
using NINJA.EShop.Shared.Messaging.MassTransit;

namespace NINJA.EShop.Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderingDb");
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
            });

            return services;
        }
    }
}