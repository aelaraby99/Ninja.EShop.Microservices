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
            services.AddScoped<ISaveChangesInterceptor,AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor,DispatchDomainEventsInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp,options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

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