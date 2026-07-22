using NINJA.EShop.Basket.API.Data;

namespace NINJA.EShop.Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart): ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);
    public class StoreBasketCommandValidator: AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotNull();
            RuleFor(x => x.Cart.UserName).NotEmpty();
            RuleFor(x => x.Cart.Items).NotNull();
        }
    }
    public class StoreBasketCommandHandler(IBasketRepository baskets)
        : ICommandHandler<StoreBasketCommand,StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command,CancellationToken cancellationToken)
        {
            var cart = command.Cart;
            var basket = await baskets.StoreBasketAsync(cart,cancellationToken);
            return new StoreBasketResult(basket.UserName);
        }
    }
}