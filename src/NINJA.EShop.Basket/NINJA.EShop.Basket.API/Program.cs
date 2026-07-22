using NINJA.EShop.Basket.API;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.AddBasketServices();
var app = builder.Build();

Console.WriteLine("Basket API is running...");
// Configure the HTTP request pipeline
app.AddBasketPipelines();
app.Run();
