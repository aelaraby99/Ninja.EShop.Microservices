using NINJA.EShop.Basket.API.Data;

namespace NINJA.EShop.Basket.API.Basket.GetBasket
{
    public record GetBasketQuery(string UserName): IQuery<GetBasketResult>;
    public record GetBasketResult(ShoppingCart Cart);
    public class GetBasketQueryHandler(IBasketRepository baskets): IQueryHandler<GetBasketQuery,GetBasketResult>
    {
        public async Task<GetBasketResult> Handle(GetBasketQuery query,CancellationToken cancellationToken)
        {
            var basket = await baskets.GetBasketAsync(query.UserName,cancellationToken);
            if (basket is null)
                throw new BasketNotFoundException(query.UserName);
            return new GetBasketResult(basket);
        }
    }
}