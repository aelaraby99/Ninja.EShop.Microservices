using NINJA.EShop.Basket.API.DTOs;

namespace NINJA.EShop.Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
public record CheckoutBasketResponse(bool IsSuccess);
public class CheckoutBasketEndpoint: ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/basket/checkout",async (CheckoutBasketRequest request,IMediator mediator) =>
		{
			var command = request.Adapt<CheckoutBasketCommand>();
			var result = await mediator.Send(command);
			var response = result.Adapt<CheckoutBasketResponse>();
			return Results.Ok(response);
		})
			.WithName("CheckoutBasket")
			.Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Checkout the basket and create an order")
			.WithDescription("This endpoint allows the user to checkout the basket and create an order. It requires a valid BasketCheckoutDto in the request body.");
	}
}