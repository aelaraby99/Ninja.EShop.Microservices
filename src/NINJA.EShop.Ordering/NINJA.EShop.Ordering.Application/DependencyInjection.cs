using Microsoft.Extensions.DependencyInjection;
using NINJA.EShop.Shared.Behaviors;
using System.Reflection;
namespace NINJA.EShop.Ordering.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MediatR handlers + validation/logging behaviors + FluentValidation validators for this assembly
            services.AddSharedMediatR(Assembly.GetExecutingAssembly());
            // Message broker (consumers + saga) is registered in Infrastructure, where the
            // EF Core outbox/inbox needs ApplicationDbContext.
            return services;
        }
    }
}