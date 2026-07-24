using NINJA.EShop.Basket.API;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.AddBasketServices();
var app = builder.Build();
// Configure the HTTP request pipeline
app.UseBasketServices();
app.Run();
