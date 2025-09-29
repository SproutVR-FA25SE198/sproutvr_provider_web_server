using Common.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Infrastructure.Configurations;
public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderItem");

        builder.Property(oi => oi.MapId)
            .IsRequired();

        // Relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // constraints
        builder.HasIndex(oi => new { oi.OrderId, oi.MapId })
            .IsUnique();
    }
}
