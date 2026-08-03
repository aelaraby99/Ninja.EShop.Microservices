using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using NINJA.EShop.Catalog.API.Data;
using NINJA.EShop.Catalog.API.Products;
using NINJA.EShop.Catalog.API.Products.StockReservation;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Exceptions.Handler;
using NINJA.EShop.Shared.Messaging.MassTransit;
namespace NINJA.EShop.Catalog.API
{
    public static class AddCatalogServicesExtensions
    {
        public static WebApplicationBuilder AddCatalogServices(this WebApplicationBuilder builder)
        {

            var programAssembly = typeof(Program).Assembly;
            // MediatR handlers + validation/logging behaviors + FluentValidation validators for this assembly
            builder.Services.AddSharedMediatR(programAssembly);
            // Carter module exposing the Product minimal-API endpoints
            builder.Services.AddCarter(configurator: cfg =>
            {
                cfg.WithModule<ProductEndpoints>();
            });
            // RabbitMQ/MassTransit bus + consumers discovered from this assembly (no outbox here, Catalog only consumes)
            builder.Services.AddMessageBroker(programAssembly,configureTransport: (context,cfg) =>
            {
                // Exchange names/types shared with every other service touching these messages.
                MessageTopology.Configure(cfg);

                // Explicit queue for the ReserveStock consumer (was auto-named "reserve-stock-consumer").
                cfg.ReceiveEndpoint("catalog.reserve-stock",e =>
                {
                    e.ConfigureConsumer<ReserveStockConsumer>(context);
                });
            });
            var martenDbConStr = builder.Configuration.GetConnectionString("MartenDb")!;
            // Marten (Postgres document store) as the Catalog read/write store
            builder.Services.AddMarten(options =>
            {
                options.Connection(martenDbConStr);
            }).UseLightweightSessions();
            // Endpoint-level rate limiting policies, applied via .RequireRateLimiting(...) on ProductEndpoints
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
                // Seeds Marten with CatalogInitialData on startup (dev only)
                builder.Services.InitializeMartenWith<CatalogInitialData>();
                // Swagger/OpenAPI generation (dev only, paired with UseSwagger/UseSwaggerUI below)
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            // Problem-details style exception handling middleware
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            // /health endpoint checks Postgres (Marten's backing store) connectivity
            builder.Services.AddHealthChecks().AddNpgSql(martenDbConStr);
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
            app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            return app;
        }
    }
}