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

        builder.Property(o => o.OrderCode)
            .IsRequired(false);

        builder.HasIndex(o => o.OrderCode).IsUnique();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.ActivationKey)
            .IsRequired(false)
            .HasColumnType("VARCHAR(100)");

        builder.Property(o => o.IsKeyActivated)
            .IsRequired()
            .HasColumnType("BOOLEAN");

        builder.Property(o => o.RepresentativeName)
        .IsRequired()
        .HasColumnType("VARCHAR(100)");

        builder.Property(o => o.RepresentativePhone)
        .IsRequired()
        .HasColumnType("VARCHAR(20)");

        builder.Property(o => o.AssignedSystemAdminId)
        .IsRequired(false);

        // Relationships
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
