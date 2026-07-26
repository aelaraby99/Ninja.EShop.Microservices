namespace NINJA.EShop.Ordering.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IApplicationDbContext context): ICommandHandler<UpdateOrderCommand,UpdateOrderResult>
{
    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command,CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.order.Id);
        var existingOrder = await context.Orders.FindAsync([orderId],cancellationToken);
        if (existingOrder is null)
            throw new OrderNotFoundException(command.order.Id);
        UpdateExistingOrder(existingOrder,command.order);
        context.Orders.Update(existingOrder);
        await context.SaveChangesAsync(cancellationToken);
        return new UpdateOrderResult(true);
    }
    private void UpdateExistingOrder(Order existingOrder,OrderDto orderDto)
    {
        var updatedShippingAddress = Address.Of(orderDto.ShippingAddress.FirstName,orderDto.ShippingAddress.LastName,orderDto.ShippingAddress.EmailAddress,orderDto.ShippingAddress.AddressLine,orderDto.ShippingAddress.Country,orderDto.ShippingAddress.State,orderDto.ShippingAddress.ZipCode);
        var updatedBillingAddress = Address.Of(orderDto.BillingAddress.FirstName,orderDto.BillingAddress.LastName,orderDto.BillingAddress.EmailAddress,orderDto.BillingAddress.AddressLine,orderDto.BillingAddress.Country,orderDto.BillingAddress.State,orderDto.BillingAddress.ZipCode);
        var updatedPayment = Payment.Of(orderDto.Payment.CardName,orderDto.Payment.CardNumber,orderDto.Payment.Expiration,orderDto.Payment.Cvv,orderDto.Payment.PaymentMethod);

        existingOrder.Update(
           orderName: OrderName.Of(orderDto.OrderName),
           shippingAddress: updatedShippingAddress,
           billingAddress: updatedBillingAddress,
           payment: updatedPayment,
           status: orderDto.Status);
    }
}