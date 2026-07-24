using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NINJA.EShop.Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderingDb");
            //services.AddDbContext<OrderingDbContext>(options =>
            //{
            //    options.UseSqlServer(connectionString);
            //});
            //services.AddScoped<IApplicationDbContext,ApplicationDbContext>();
            return services;
        }
    }
}