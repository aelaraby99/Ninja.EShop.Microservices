namespace NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCancelled;

public class MarkOrderAsCancelledCommandHandler(IApplicationDbContext context,ILogger<MarkOrderAsCancelledCommandHandler> logger): ICommandHandler<MarkOrderAsCancelledCommand,MarkOrderAsCancelledResult>
{
    public async Task<MarkOrderAsCancelledResult> Handle(MarkOrderAsCancelledCommand command,CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.OrderId);
        var order = await context.Orders.FindAsync([orderId],cancellationToken);
        if (order == null)
            throw new OrderNotFoundException(command.OrderId);
        logger.LogInformation("Cancelling order {OrderId}: {Reason}",command.OrderId,command.Reason);
        order.MarkAsCancelled();
        await context.SaveChangesAsync(cancellationToken);
        return new MarkOrderAsCancelledResult(true);
    }
}
