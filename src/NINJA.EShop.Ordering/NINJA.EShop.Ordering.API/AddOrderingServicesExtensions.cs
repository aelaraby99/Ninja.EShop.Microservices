namespace NINJA.EShop.Ordering.API
{
    public static class AddOrderingServicesExtensions
    {
        public static WebApplicationBuilder AddOrderingServices(this WebApplicationBuilder builder)
        {
            return builder;
        }
        public static WebApplication UseOrderingServices(this WebApplication app)
        {
            return app;
        }
    }
}
