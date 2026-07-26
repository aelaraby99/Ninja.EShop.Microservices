namespace NINJA.EShop.Ordering.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(OrderDto order): ICommand<UpdateOrderResult>;
public record UpdateOrderResult(bool IsSuccess);
public class UpdateOrderValidator: AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.order).NotNull().WithMessage("Order cannot be null.");
        RuleFor(x => x.order.Id).NotEmpty().WithMessage("Order Id cannot be empty.");
        RuleFor(x => x.order.CustomerId).NotEmpty().WithMessage("Customer Id cannot be empty.");
        RuleFor(x => x.order.OrderName).NotEmpty().WithMessage("Order Name cannot be empty.");
        RuleFor(x => x.order.ShippingAddress).NotNull().WithMessage("Shipping Address cannot be null.");
        RuleFor(x => x.order.BillingAddress).NotNull().WithMessage("Billing Address cannot be null.");
        RuleFor(x => x.order.Payment).NotNull().WithMessage("Payment cannot be null.");
        RuleFor(x => x.order.OrderItems).NotEmpty().WithMessage("Order Items cannot be empty.");
    }
}