using MassTransit;
using NINJA.EShop.Ordering.Application.Orders.Commands.CreateOrder;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Ordering.Application.Orders.EventHandlers.Integration;

public class BasketCheckoutEventHandler(ISender sender,ILogger<BasketCheckoutEventHandler> logger): IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        logger.LogInformation("Integration Evenet handled: {IntegrationEvent}",context.Message.GetType().Name);
        var command = MapToCreateOrderCommand(context.Message);
        await sender.Send(command,context.CancellationToken);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
    {
        var addressDto = new AddressDto(message.FirstName,message.LastName,message.EmailAddress,message.AddressLine,message.Country,message.State,message.ZipCode);
        var paymentDto = new PaymentDto(message.CardName,message.CardNumber,message.Expiration,message.CVV,message.PaymentMethod);
        var orderId = Guid.NewGuid();
        var orderItems = message.Items
            .Select(item => new OrderItemDto(orderId,item.ProductId,item.Quantity,item.Price)).ToList();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: message.CustomerId,
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Status: OrderStatus.Pending,
            OrderItems: orderItems);
        return new CreateOrderCommand(orderDto);
    }
}