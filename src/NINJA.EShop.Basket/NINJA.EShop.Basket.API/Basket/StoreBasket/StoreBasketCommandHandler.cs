using NINJA.EShop.Basket.API.Data;
using NINJA.EShop.Discount.Grpc.Protos;

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
            RuleFor(x => x.Cart.CustomerId).NotEmpty();
            RuleFor(x => x.Cart.Items).NotNull();
        }
    }
    public class StoreBasketCommandHandler(IBasketRepository baskets,DiscountProtoService.DiscountProtoServiceClient discountProto)
        : ICommandHandler<StoreBasketCommand,StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command,CancellationToken cancellationToken)
        {
            await DeductDiscount(command,cancellationToken);
            await baskets.StoreBasketAsync(command.Cart,cancellationToken);
            return new StoreBasketResult(command.Cart.UserName);
        }

        private async Task DeductDiscount(StoreBasketCommand command,CancellationToken cancellationToken)
        {
            foreach (var item in command.Cart.Items)
            {
                var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName },cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}