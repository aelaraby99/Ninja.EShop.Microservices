using NINJA.EShop.Basket.API.Basket.GetBasket;
namespace NINJA.EShop.Basket.API.Basket
{
    //public record GetBasketRequest(string UserName);
    public record GetBasketResponse(ShoppingCart Cart);
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
        }
    }
}