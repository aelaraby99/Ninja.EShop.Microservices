using NINJA.EShop.Discount.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDiscountServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDiscountServices();
app.Run();
