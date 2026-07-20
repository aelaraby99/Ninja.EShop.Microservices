using NINJA.EShop.Catalog.API;
var builder = WebApplication.CreateBuilder(args);
builder.AddCatalogServices();
var app = builder.Build();
app.AddCatalogServices();
app.Run();
