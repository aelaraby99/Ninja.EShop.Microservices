using JasperFx;
using Microsoft.AspNetCore.RateLimiting;
using Ninja.EShop.Catalog.API.Products.CreateProduct;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter(configurator: cfg =>
{
    cfg.WithModule<CreateProductEndpoint>();
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapCarter();
app.Run();
