using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Infrastructure.Data.Database;
public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    { 
    }
    // DbSets
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Add Outbox Pattern
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
