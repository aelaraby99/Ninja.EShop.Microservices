using NINJA.EShop.Catalog.API.Products.CreateProduct;
using NINJA.EShop.Catalog.API.Products.DeleteProduct;
using NINJA.EShop.Catalog.API.Products.GetProductByCategory;
using NINJA.EShop.Catalog.API.Products.GetProductById;
using NINJA.EShop.Catalog.API.Products.GetProducts;
using NINJA.EShop.Catalog.API.Products.UpdateProduct;
using NINJA.EShop.Shared.Results;
namespace NINJA.EShop.Catalog.API.Products
{
    public record GetProductsRequest(int PageNumber = 1,int PageSize = 10);
    public record GetProductsResponse(IEnumerable<Product> Products);
    public record CreateProductRequest(string Name,List<string> Category,string Description,string ImageFile,decimal Price);
    public record UpdateProductRequest(Guid Id,string Name,List<string> Category,string Description,string ImageFile,decimal Price);
    public record UpdateProductResponse(bool IsSuccess);
    public record DeleteProductResponse(bool IsSuccess);
    public record CreateProductResponse(Guid Id);
    public record GetProductByIdResponse(Product Product);
    public record GetProductByCategoryResponse(IEnumerable<Product> Products);
    public class ProductEndpoints: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products",async ([AsParameters] GetProductsRequest request,ISender sender) =>
            {
                var query = request.Adapt<GeProductsQuery>();
                var result = await sender.Send(query);
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
                var response = new CreateProductResponse(result.Id);
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

            app.MapGet("/products/category/{category}",async (string category,ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));
                var response = result.Adapt<GetProductByCategoryResponse>();
                return Results.Ok(response);
            }).RequireRateLimiting("default")
            .WithTags("Products")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Retrieves products by category")
            .WithDescription("Retrieves a list of products that belong to the specified category.");

            app.MapPut("/products",async (UpdateProductRequest request,ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductResponse>();
                return Results.Ok(response);
            }).RequireRateLimiting("default")
            .WithTags("Products")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updates an existing product")
            .WithDescription("Updates the details of an existing product with the specified information.");

            app.MapDelete("/products/{id:guid}",async (Guid id,ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id));
                var response = result.Adapt<DeleteProductResponse>();
                return Results.Ok(response);
            }).RequireRateLimiting("default")
            .WithTags("Products")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete a product by ID")
            .WithDescription("Delete the details of a product by its unique identifier.");
        }
    }
}