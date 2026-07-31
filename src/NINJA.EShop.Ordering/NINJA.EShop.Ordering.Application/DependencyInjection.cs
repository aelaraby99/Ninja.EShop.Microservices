using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Messaging.MassTransit;
using System.Reflection;
namespace NINJA.EShop.Ordering.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            services.AddMessageBroker(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}