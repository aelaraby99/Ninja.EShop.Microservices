using FluentValidation;

namespace NINJA.EShop.Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderResult(Guid Id);
public record CreateOrderCommand(OrderDto Order): ICommand<CreateOrderResult>;
public class CreatOrderCommandValidator: AbstractValidator<CreateOrderCommand>
{
    public CreatOrderCommandValidator()
    {
        RuleFor(x => x.Order).NotNull();
        RuleFor(x => x.Order.OrderName).NotEmpty();
        RuleFor(x => x.Order.CustomerId).NotEmpty();
        RuleFor(x => x.Order.OrderItems).NotEmpty();
        RuleForEach(x => x.Order.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}
public class OrderItemDtoValidator: AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}