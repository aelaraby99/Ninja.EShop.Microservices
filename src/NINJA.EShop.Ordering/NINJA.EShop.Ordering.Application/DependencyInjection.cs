using Microsoft.Extensions.DependencyInjection;
using NINJA.EShop.Shared.Behaviors;
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
                cfg.AddBehavior(typeof(LoggingBehavior<,>));
            });
            return services;
        }
    }
}