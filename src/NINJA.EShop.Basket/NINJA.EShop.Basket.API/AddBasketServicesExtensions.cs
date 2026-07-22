using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using NINJA.EShop.Basket.API.Basket;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Exceptions.Handler;
namespace NINJA.EShop.Basket.API
{
    public static class AddBasketServicesExtensions
    {
        public static WebApplicationBuilder AddBasketServices(this WebApplicationBuilder builder)
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
                cfg.WithModule<BasketEndpoints>();
            });
            var martenDbConStr = builder.Configuration.GetConnectionString("MartenDb")!;
            builder.Services.AddMarten(options =>
            {
                options.Connection(martenDbConStr);
                options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
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
            });
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            builder.Services.AddHealthChecks();
            return builder;
        }
        public static WebApplication AddBasketPipelines(this WebApplication app)
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
            app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            return app;
        }
    }
}