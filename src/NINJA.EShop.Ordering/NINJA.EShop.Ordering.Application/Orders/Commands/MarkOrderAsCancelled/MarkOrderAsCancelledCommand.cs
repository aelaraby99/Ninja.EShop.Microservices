namespace NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCancelled;

public record MarkOrderAsCancelledCommand(Guid OrderId, string Reason): ICommand<MarkOrderAsCancelledResult>;
public record MarkOrderAsCancelledResult(bool IsSuccess);
