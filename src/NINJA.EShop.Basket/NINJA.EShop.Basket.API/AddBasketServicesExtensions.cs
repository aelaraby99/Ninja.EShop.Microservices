using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using NINJA.EShop.Basket.API.Basket;
using NINJA.EShop.Basket.API.Data;
using NINJA.EShop.Discount.Grpc.Protos;
using NINJA.EShop.Shared.Behaviors;
using NINJA.EShop.Shared.Exceptions.Handler;
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

        private static void GrpcServices(WebApplicationBuilder builder)
        {
            builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
            {
                options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
            });
        }

        private static void CrossCuttingServices(WebApplicationBuilder builder,string redisDbConStr,string martenDbConStr)
        {
            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            builder.Services.AddHealthChecks()
                .AddRedis(redisDbConStr,name: "Redis")
                .AddNpgSql(martenDbConStr,name: "Postgres");
        }

        private static void DataServices(WebApplicationBuilder builder,string redisDbConStr,string martenDbConStr)
        {
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
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisDbConStr;
            });
        }

        private static void ApplicationServices(WebApplicationBuilder builder)
        {
            var programAssembly = typeof(Program).Assembly;
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(programAssembly);
                cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
            builder.Services.AddCarter(configurator: cfg =>
            {
                cfg.WithModule<BasketEndpoints>();
            });
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
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