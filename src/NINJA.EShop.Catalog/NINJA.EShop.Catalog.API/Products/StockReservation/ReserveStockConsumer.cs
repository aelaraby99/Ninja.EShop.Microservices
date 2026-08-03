using MassTransit;
using NINJA.EShop.Catalog.API.Data;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Catalog.API.Products.StockReservation;

// Stock reservation step for the Ordering saga: checks all line items have sufficient stock
// before decrementing anything, or reports failure so the saga can cancel the order
// (compensating action) instead of leaving a partially-decremented product.
public class ReserveStockConsumer(IDocumentSession session,ILogger<ReserveStockConsumer> logger): IConsumer<ReserveStock>
{
    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        var messageId = context.MessageId
            ?? throw new InvalidOperationException($"{nameof(ReserveStock)} arrived without a MessageId; cannot dedupe safely.");

        // Marten has no built-in inbox; redelivery of an already-handled ReserveStock must not
        // decrement stock a second time (or re-fail/re-succeed a reservation that already settled).
        if (await session.LoadAsync<ProcessedInboxMessage>(messageId,context.CancellationToken) is not null)
        {
            logger.LogInformation("Duplicate delivery of {MessageId} for {Event}, already processed - skipping",messageId,nameof(ReserveStock));
            return;
        }

        var message = context.Message;
        var productIds = message.Items.Select(i => i.ProductId).ToList();
        var products = await session.Query<Product>()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(context.CancellationToken);

        foreach (var item in message.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is null || product.Stock < item.Quantity)
            {
                logger.LogWarning("Stock reservation failed for order {OrderId}: product {ProductId}",message.OrderId,item.ProductId);
                session.Store(new ProcessedInboxMessage { Id = messageId,ProcessedAt = DateTime.UtcNow });
                await session.SaveChangesAsync(context.CancellationToken);
                await context.Publish(new StockReservationFailed(message.OrderId,$"Insufficient stock for product {item.ProductId}"));
                return;
            }
        }

        foreach (var item in message.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.Stock -= item.Quantity;
            session.Update(product);
        }

        session.Store(new ProcessedInboxMessage { Id = messageId,ProcessedAt = DateTime.UtcNow });
        await session.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Stock reserved for order {OrderId}",message.OrderId);
        await context.Publish(new StockReserved(message.OrderId));
    }
}
