using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace NINJA.EShop.Shared.Behaviors;

public static class DependencyInjection
{
    // Cross-cutting CQRS wiring for a service's own assembly: MediatR handlers, the shared
    // validation/logging pipeline behaviors, and the FluentValidation validators that back them.
    // Registering all three together means a service can't add ValidationBehaviors without also
    // registering the IValidator<T> implementations it depends on.
    public static IServiceCollection AddSharedMediatR(this IServiceCollection services,Assembly assembly)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly); // discovers command/query/notification handlers in the assembly
            cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>)); // validates commands via FluentValidation before the handler runs
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)); // logs request/response around each handled command/query
        });
        services.AddValidatorsFromAssembly(assembly); // registers IValidator<T> for every AbstractValidator<T> in the assembly
        return services;
    }
}
