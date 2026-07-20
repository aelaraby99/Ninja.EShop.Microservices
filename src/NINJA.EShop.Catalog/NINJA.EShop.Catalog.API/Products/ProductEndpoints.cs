using NINJA.EShop.Catalog.API.Products.CreateProduct;
using NINJA.EShop.Catalog.API.Products.GetProductById;
using NINJA.EShop.Catalog.API.Products.GetProducts;
namespace NINJA.EShop.Catalog.API.Products
{
    public record GetProductsResponse(IEnumerable<Product> Products);
    public record CreateProductRequest(string Name,List<string> Category,string Description,string ImageFile,decimal Price);
    public record CreateProductResponse(Guid Id);
    public record GetProductByIdResponse(Product Product);
    public class ProductEndpoints: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products",async (ISender sender) =>
            {
                var result = await sender.Send(new GeProductsQuery());
                var response = result.Adapt<GetProductsResponse>();
                return Results.Ok(response);
            }).RequireRateLimiting("default")
              .WithTags("Products")
              .Produces<GetProductsResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Retrieves a list of products")
              .WithDescription("Retrieves a list of all products in the catalog.");

            app.MapPost("/products",async (CreateProductRequest request,ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}",response);
            }).RequireRateLimiting("create-product")
            .WithTags("Products")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new product")
            .WithDescription("Creates a new product with the specified details.");

            app.MapGet("/products/{id:guid}",async (Guid id,ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                if (result is null)
                {
                    return Results.NotFound();
                }
                var response = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response);
            }).RequireRateLimiting("default")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieves a product by ID")
            .WithDescription("Retrieves the details of a product by its unique identifier.");
        }
    }
}