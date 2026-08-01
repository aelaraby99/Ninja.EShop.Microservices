using MassTransit;
using NINJA.EShop.Shared.Messaging.Events;

namespace NINJA.EShop.Catalog.API.Products.StockReservation;

// Simulated stock reservation step for the Ordering saga: decrements Product.Stock for
// every line item, or reports failure so the saga can cancel the order (compensating action).
public class ReserveStockConsumer(IDocumentSession session,ILogger<ReserveStockConsumer> logger): IConsumer<ReserveStock>
{
    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
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

        await session.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Stock reserved for order {OrderId}",message.OrderId);
        await context.Publish(new StockReserved(message.OrderId));
    }
}
