using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NINJA.EShop.Shared.Exceptions.Handler;

namespace NINJA.EShop.Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingApiServices(this IServiceCollection services,IConfiguration configuration)
    {
        // Carter module discovery for the minimal-API endpoints (Orders, etc.)
        services.AddCarter();
        // Problem-details style exception handling middleware
        services.AddExceptionHandler<CustomExceptionHandler>();
        // /health endpoint checks SQL Server (OrderingDb) connectivity
        services.AddHealthChecks().AddSqlServer(configuration.GetConnectionString("OrderingDb")!);
        return services;
    }
    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();
        app.UseExceptionHandler(options => { });
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        return app;
    }
}