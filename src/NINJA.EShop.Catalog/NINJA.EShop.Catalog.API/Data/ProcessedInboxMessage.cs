namespace NINJA.EShop.Catalog.API.Data;

// Marten has no built-in EF-outbox-style inbox/dedupe, unlike Ordering's InboxState.
// Consumers that must be idempotent under redelivery track their own processed message ids
// here, Id'd by the MassTransit MessageId, written in the same session/transaction as the
// business change so both commit or neither does.
public class ProcessedInboxMessage
{
    public Guid Id { get; set; }
    public DateTime ProcessedAt { get; set; }
}
