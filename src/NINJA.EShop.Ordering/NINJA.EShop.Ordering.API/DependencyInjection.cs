namespace NINJA.EShop.Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingApiServices(this IServiceCollection services)
    {
        return services;
    }
    public static async Task<WebApplication> UseApiServices(this WebApplication app)
    {
        //app.MapCarter();
        if (app.Environment.IsDevelopment())
        {
            await app.InitialiseDatabaseAsync();
        }
        return app;
    }
}