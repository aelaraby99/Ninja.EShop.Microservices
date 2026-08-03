using MassTransit;

namespace NINJA.EShop.Ordering.Application.Sagas.OrderProcessing;

public class OrderProcessingState: SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public DateTime SubmittedAt { get; set; }
}
