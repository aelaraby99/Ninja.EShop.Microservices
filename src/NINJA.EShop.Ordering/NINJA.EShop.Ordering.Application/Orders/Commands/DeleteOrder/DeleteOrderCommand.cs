namespace NINJA.EShop.Ordering.Application.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId): ICommand<DeleteOrderResult>;
public record DeleteOrderResult(bool IsSuccess);
public class DeleteOrderValidator: AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id cannot be empty.");
    }
}