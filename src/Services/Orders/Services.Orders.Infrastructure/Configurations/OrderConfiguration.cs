using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Infrastructure.Configurations;
public class OrderConfiguration : BaseEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);
        builder.ToTable("Order");

        builder.Property(o => o.OrganizationId)
            .IsRequired();

        builder.Property(o => o.TotalMoneyAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TransactionCode)
            .IsRequired(false)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(o => o.PaymentMethod)
            .IsRequired(false)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.Bank)
            .IsRequired(false)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.BundleUrl)
            .IsRequired(false)
            .HasConversion<string>()
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);

        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
