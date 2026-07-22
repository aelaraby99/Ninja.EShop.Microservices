using NINJA.EShop.Basket.API.Basket.DeleteBasket;
using NINJA.EShop.Basket.API.Basket.GetBasket;
using NINJA.EShop.Basket.API.Basket.StoreBasket;
namespace NINJA.EShop.Basket.API.Basket
{
    //public record GetBasketRequest(string UserName);
    public record GetBasketResponse(ShoppingCart Cart);
    public record StoreBasketRequest(ShoppingCart Cart);
    public record StoreBasketResponse(string UserName);
    public record DeleteBasketRequest(string UserName);
    public record DeleteBasketResponse(bool IsSuccess);
    public class BasketEndpoints: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}",async (string userName,ISender sender,CancellationToken cancellationToken) =>
            {
                var query = new GetBasketQuery(userName);
                var result = await sender.Send(query,cancellationToken);
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            }).WithName("GetBasket")
            .WithTags("Basket")
            .WithSummary("Get the shopping cart for a user")
            .WithDescription("This endpoint retrieves the shopping cart for a specific user based on their username.")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            app.MapPost("/basket",async (StoreBasketRequest request,ISender sender,CancellationToken cancellationToken) =>
            {
                var command = request.Adapt<StoreBasketCommand>();
                var result = await sender.Send(command,cancellationToken);
                var response = result.Adapt<StoreBasketResponse>();
                return Results.Created($"/basket/{response.UserName}",response);
            }).WithName("StoreBasket")
            .WithTags("Basket")
            .WithSummary("Store the shopping cart for a user")
            .WithDescription("This endpoint stores the shopping cart for a specific user.")
            .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            app.MapDelete("/basket/{userName}",async (string userName,ISender sender,CancellationToken cancellationToken) =>
            {
                var command = new DeleteBasketCommand(userName);
                var result = await sender.Send(command,cancellationToken);
                var response = result.Adapt<DeleteBasketResponse>();
                return Results.Ok(response);
            }).WithName("DeleteBasket")
            .WithTags("Basket")
            .WithSummary("Delete the shopping cart for a user")
            .WithDescription("This endpoint deletes the shopping cart for a specific user based on their username.")
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}