namespace NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCompleted;

public class MarkOrderAsCompletedCommandHandler(IApplicationDbContext context): ICommandHandler<MarkOrderAsCompletedCommand,MarkOrderAsCompletedResult>
{
    public async Task<MarkOrderAsCompletedResult> Handle(MarkOrderAsCompletedCommand command,CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.OrderId);
        var order = await context.Orders.FindAsync([orderId],cancellationToken);
        if (order == null)
            throw new OrderNotFoundException(command.OrderId);
        order.MarkAsCompleted();
        await context.SaveChangesAsync(cancellationToken);
        return new MarkOrderAsCompletedResult(true);
    }
}
