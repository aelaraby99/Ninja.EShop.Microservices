using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using NINJA.EShop.Basket.API.Basket;
using NINJA.EShop.Basket.API.Basket.CheckoutBasket;
using NINJA.EShop.Basket.API.Data;
using NINJA.EShop.Discount.Grpc.Protos;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Exceptions.Handler;
using NINJA.EShop.Shared.Messaging.MassTransit;
namespace NINJA.EShop.Basket.API
{
    public static class AddBasketServicesExtensions
    {
        public static WebApplicationBuilder AddBasketServices(this WebApplicationBuilder builder)
        {
            var martenDbConStr = builder.Configuration.GetConnectionString("MartenDb")!;
            var redisDbConStr = builder.Configuration.GetConnectionString("Redis")!;
            ApplicationServices(builder);
            DataServices(builder,redisDbConStr,martenDbConStr);
            CrossCuttingServices(builder,redisDbConStr,martenDbConStr);
            // RabbitMQ/MassTransit bus (no consumers assembly: Basket only publishes BasketCheckoutEvent).
            // Still declares the shared topology so its publish-side exchange name/type match what
            // Ordering's consumer expects - names must agree across services touching a message.
            builder.Services.AddMessageBroker(configureTransport: (_,cfg) => MessageTopology.Configure(cfg));
            GrpcServices(builder);
            return builder;
        }

        public static WebApplication UseBasketServices(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
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

        private static void GrpcServices(WebApplicationBuilder builder)
        {
            // Discount gRPC client used synchronously from Checkout to price basket items.
            // Dev only: skips TLS certificate validation for the local self-signed dev cert.
            var grpcClientBuilder = builder.Environment.IsDevelopment()
                ? builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
                {
                    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
                }).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                })
                : builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
                {
                    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
                });

            // Circuit breaker + retry + timeout around the synchronous Discount gRPC dependency,
            // so a Discount outage fails fast instead of hanging/cascading into Basket requests.
            grpcClientBuilder.AddStandardResilienceHandler(options =>
            {
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
                options.Retry.MaxRetryAttempts = 3;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
            });
        }

        private static void CrossCuttingServices(WebApplicationBuilder builder,string redisDbConStr,string martenDbConStr)
        {
            // Problem-details style exception handling middleware
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            // /health endpoint checks both Redis (cache) and Postgres (Marten's backing store)
            builder.Services.AddHealthChecks()
                .AddRedis(redisDbConStr,name: "Redis")
                .AddNpgSql(martenDbConStr,name: "Postgres");
        }

        private static void DataServices(WebApplicationBuilder builder,string redisDbConStr,string martenDbConStr)
        {
            // Marten (Postgres document store) as the ShoppingCart backing store, keyed by UserName
            builder.Services.AddMarten(options =>
            {
                options.Connection(martenDbConStr);
                options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
            }).UseLightweightSessions();
            /// Instead Use Scrutor to decorate the BasketRepository with CacheBasketRepository
            ///builder.Services.AddScoped<IBasketRepository>(provider =>
            ///{
            ///    var basketRepository = provider.GetRequiredService<BasketRepository>();
            ///    return new CacheBasketRepository(basketRepository,provider.GetService<IDistributedCache>());
            ///});
            builder.Services.AddScoped<IBasketRepository,BasketRepository>();
            builder.Services.Decorate<IBasketRepository,CacheBasketRepository>();
            // Redis-backed IDistributedCache, used by CacheBasketRepository above
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisDbConStr;
            });
        }

        private static void ApplicationServices(WebApplicationBuilder builder)
        {
            var programAssembly = typeof(Program).Assembly;
            // MediatR handlers + validation/logging behaviors + FluentValidation validators for this assembly
            builder.Services.AddSharedMediatR(programAssembly);
            // Carter modules exposing the Basket and Checkout minimal-API endpoints
            builder.Services.AddCarter(configurator: cfg =>
            {
                cfg.WithModule<BasketEndpoints>();
                cfg.WithModule<CheckoutBasketEndpoint>();
            });
            if (builder.Environment.IsDevelopment())
            {
                // Swagger/OpenAPI generation (dev only, paired with UseSwagger/UseSwaggerUI below)
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            // Demo rate-limiter policy: not currently applied to any endpoint via RequireRateLimiting.
            // Intentionally left as-is for now; rate limiting is planned to move to the gateway later.
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
        }
    }
}