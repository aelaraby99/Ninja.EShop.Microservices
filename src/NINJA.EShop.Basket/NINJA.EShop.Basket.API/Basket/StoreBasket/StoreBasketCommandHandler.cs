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
    public class StoreBasketCommandHandler
        : ICommandHandler<StoreBasketCommand,StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand request,CancellationToken cancellationToken)
        {
            var cart = request.Cart;
            // Store and Update Cache
            return new StoreBasketResult("true");
        }
    }
}