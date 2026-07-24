using NINJA.EShop.Ordering.API;
using NINJA.EShop.Ordering.Application;
using NINJA.EShop.Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddOrderingApiServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();
