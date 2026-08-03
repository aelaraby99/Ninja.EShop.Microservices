using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCancelled;
using NINJA.EShop.Ordering.Application.Orders.Commands.MarkOrderAsCompleted;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Ordering.Application.Sagas.OrderProcessing;

// Orchestrates order confirmation: OrderCreated -> reserve stock in Catalog -> Completed/Cancelled,
// with Cancelled acting as the compensating outcome when stock reservation fails.
public class OrderProcessingStateMachine: MassTransitStateMachine<OrderProcessingState>
{
    public State AwaitingStockConfirmation { get; private set; } = default!;
    public State Completed { get; private set; } = default!;
    public State Cancelled { get; private set; } = default!;

    public Event<OrderCreatedIntegrationEvent> OrderCreated { get; private set; } = default!;
    public Event<StockReserved> StockReservedEvent { get; private set; } = default!;
    public Event<StockReservationFailed> StockReservationFailedEvent { get; private set; } = default!;

    public OrderProcessingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated,e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => StockReservedEvent,e => e.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => StockReservationFailedEvent,e => e.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
            When(OrderCreated)
                .Then(ctx =>
                {
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.SubmittedAt = DateTime.UtcNow;
                })
                // ctx.Init<T>() builds the message via reflection over a default constructor, which
                // ReserveStock (a positional record) doesn't have - it throws ArgumentException at
                // runtime ("No default constructor available"). We already have a fully-formed
                // instance, so publish it directly instead of routing it through Init<T>.
                .PublishAsync(ctx => Task.FromResult(new ReserveStock(ctx.Message.OrderId,ctx.Message.Items)))
                .TransitionTo(AwaitingStockConfirmation));

        During(AwaitingStockConfirmation,
            When(StockReservedEvent)
                .ThenAsync(CompleteOrderAsync)
                .TransitionTo(Completed)
                .Finalize(),
            When(StockReservationFailedEvent)
                .ThenAsync(CancelOrderAsync)
                .TransitionTo(Cancelled)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    private static async Task CompleteOrderAsync(BehaviorContext<OrderProcessingState,StockReserved> context)
    {
        var sender = context.GetPayload<IServiceProvider>().GetRequiredService<MediatR.ISender>();
        await sender.Send(new MarkOrderAsCompletedCommand(context.Saga.CorrelationId));
    }

    private static async Task CancelOrderAsync(BehaviorContext<OrderProcessingState,StockReservationFailed> context)
    {
        var sender = context.GetPayload<IServiceProvider>().GetRequiredService<MediatR.ISender>();
        await sender.Send(new MarkOrderAsCancelledCommand(context.Saga.CorrelationId,context.Message.Reason));
    }
}
