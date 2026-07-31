namespace NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCompleted;

public record MarkOrderAsCompletedCommand(Guid OrderId): ICommand<MarkOrderAsCompletedResult>;
public record MarkOrderAsCompletedResult(bool IsSuccess);
