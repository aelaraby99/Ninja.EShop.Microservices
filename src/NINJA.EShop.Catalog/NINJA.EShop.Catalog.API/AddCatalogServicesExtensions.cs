using Microsoft.AspNetCore.RateLimiting;
using NINJA.EShop.Catalog.API.Data;
using NINJA.EShop.Catalog.API.Products;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Exceptions.Handler;
namespace NINJA.EShop.Catalog.API
{
    public static class AddCatalogServicesExtensions
    {
        public static WebApplicationBuilder AddCatalogServices(this WebApplicationBuilder builder)
        {

            var programAssembly = typeof(Program).Assembly;
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(programAssembly);
                cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            builder.Services.AddValidatorsFromAssembly(programAssembly);
            builder.Services.AddCarter(configurator: cfg =>
            {
                cfg.WithModule<ProductEndpoints>();
            });
            builder.Services.AddMarten(options =>
            {
                options.Connection(builder.Configuration.GetConnectionString("MartenDb")!);
            }).UseLightweightSessions();
            builder.Services.AddRateLimiter(options =>
            {
                // Default policy
                options.AddFixedWindowLimiter("default",opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });
                // Strict policy for creating products
                options.AddFixedWindowLimiter("create-product",opt =>
                {
                    opt.PermitLimit = 3;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });
            });
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.InitializeMartenWith<CatalogInitialData>();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            return builder;
        }
        public static WebApplication AddCatalogServices(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.MapCarter();
            app.UseExceptionHandler(options => { });
            return app;
        }
    }
}