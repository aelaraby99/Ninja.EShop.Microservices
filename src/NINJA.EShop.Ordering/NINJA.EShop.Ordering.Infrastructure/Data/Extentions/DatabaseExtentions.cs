using Microsoft.AspNetCore.Builder;

namespace NINJA.EShop.Ordering.Infrastructure.Data.Extentions;

public static class DatabaseExtentions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await context.SeedAsync();
    }
    private static async Task SeedAsync(this ApplicationDbContext context)
    {
        await context.SeedCustomerAsync();
        await context.SeedProductAsync();
        //await context.SeedOrderandItemsAsync();
    }
    private static async Task SeedCustomerAsync(this ApplicationDbContext context)
    {
        if (!await context.Customers.AnyAsync())
        {
            await context.Customers.AddRangeAsync(InitialData.Customers);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedProductAsync(this ApplicationDbContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            await context.Products.AddRangeAsync(InitialData.Products);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedOrderandItemsAsync(this ApplicationDbContext context)
    {
        if (!await context.Orders.AnyAsync())
        {
            await context.Orders.AddRangeAsync(InitialData.OrdersWithItems);
            await context.SaveChangesAsync();
        }
    }
}