using NINJA.EShop.Ordering.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddOrderingServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOrderingServices();
app.Run();
