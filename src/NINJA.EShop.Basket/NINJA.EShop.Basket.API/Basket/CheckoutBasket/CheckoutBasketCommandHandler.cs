using MassTransit;
using NINJA.EShop.Basket.API.Data;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Basket.API.Basket.CheckoutBasket;

public class CheckoutBasketCommandHandler
    (IBasketRepository repository,IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand,CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command,CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasketAsync(command.BasketCheckoutDto.UserName,cancellationToken);
        if (basket == null)
            return new CheckoutBasketResult(false);

        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.CustomerId = basket.CustomerId;
        eventMessage.TotalPrice = basket.TotalPrice;
        eventMessage.Items = basket.Items
            .Select(item => new BasketCheckoutItem(item.ProductId, item.Quantity, item.Price))
            .ToList();
        await publishEndpoint.Publish(eventMessage,cancellationToken);
        await repository.DeleteBasketAsync(command.BasketCheckoutDto.UserName,cancellationToken);
        return new CheckoutBasketResult(true);
    }
}