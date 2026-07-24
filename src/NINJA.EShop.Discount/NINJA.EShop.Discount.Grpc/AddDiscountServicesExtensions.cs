using Microsoft.EntityFrameworkCore;
using NINJA.EShop.Discount.Grpc.Data;
using NINJA.EShop.Discount.Grpc.Services;

namespace NINJA.EShop.Discount.Grpc;

public static class AddDiscountServicesExtensions
{
    public static WebApplicationBuilder AddDiscountServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");
        builder.Services.AddDbContext<DiscountContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        return builder;
    }

    public static WebApplication UseDiscountServices(this WebApplication app)
    {
        app.AutoMigrateDatabase();
        app.MapGrpcService<DiscountService>();
        return app;
    }

    private static WebApplication AutoMigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        DiscountContext dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        dbContext.Database.Migrate();
        return app;
    }
}