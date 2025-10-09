using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Infrastructure.Configurations;
public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderItem");

        builder.Property(oi => oi.MapId)
            .IsRequired();

        builder.Property(oi => oi.MapCode)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(oi => oi.MapName)
            .HasColumnType("varchar(300)")
            .HasMaxLength(100);

        builder.Property(m => m.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.ImageUrl)
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);

        // Relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        // constraints
        builder.HasIndex(oi => new { oi.OrderId, oi.MapId })
            .IsUnique();
    }
}
