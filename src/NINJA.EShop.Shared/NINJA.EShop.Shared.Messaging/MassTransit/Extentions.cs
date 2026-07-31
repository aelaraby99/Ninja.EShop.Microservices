using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NINJA.EShop.Shared.Messaging.Options;
using System.Reflection;

namespace NINJA.EShop.Shared.Messaging.MassTransit;

public static class Extentions
{
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        Assembly? consumersAssembly = null,
        Action<IBusRegistrationConfigurator>? configureBus = null)
    {
        services.AddOptions<RabbitMqOptions>()
            .BindConfiguration(RabbitMqOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();
            if (consumersAssembly is not null)
                bus.AddConsumers(consumersAssembly);

            configureBus?.Invoke(bus);

            bus.UsingRabbitMq((context,cfg) =>
            {
                RabbitMqOptions options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                cfg.Host(options.Host,options.Port,options.VirtualHost,host =>
                {
                    host.Username(options.UserName);
                    host.Password(options.Password);
                });
                cfg.PrefetchCount = options.PrefetchCount;

                cfg.UseMessageRetry(r =>
                    r.Exponential(
                        5,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(5)));

                cfg.UseDelayedRedelivery(r =>
                    r.Intervals(
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(15)));

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.ActiveThreshold = 10;
                    cb.TripThreshold = 15;
                    cb.ResetInterval = TimeSpan.FromMinutes(1);
                });

                cfg.UseInMemoryOutbox(context);
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}