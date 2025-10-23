using MassTransit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Services.Bundles.Infrastructure.Data.Database;
public class BundleDbContext : DbContext
{
    public BundleDbContext(DbContextOptions<BundleDbContext> options) : base(options)
    {
    }

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
