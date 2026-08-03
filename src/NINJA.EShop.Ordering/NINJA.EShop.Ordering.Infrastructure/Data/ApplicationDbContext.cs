using MassTransit;
using NINJA.EShop.Ordering.Application.Sagas.OrderProcessing;

namespace NINJA.EShop.Ordering.Infrastructure.Data;

public class ApplicationDbContext: DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderProcessingState> OrderProcessingStates => Set<OrderProcessingState>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // MassTransit transactional outbox: buffers publishes made in this scope until
        // SaveChanges commits, and dedupes consumed messages via InboxState (inbox pattern).
        base.OnModelCreating(builder);
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();

        // Saga persistence for OrderProcessingStateMachine (OrderCreated -> ReserveStock -> Completed/Cancelled)
        builder.Entity<OrderProcessingState>(entity =>
        {
            entity.HasKey(x => x.CorrelationId);
            entity.Property(x => x.CurrentState).HasMaxLength(64);
        });
    }
}