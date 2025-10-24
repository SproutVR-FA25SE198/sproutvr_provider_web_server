using Common.Domain.Entities;
using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Infrastructure.Configurations;
public class PaymentTransactionConfiguration : BaseEntityConfiguration<PaymentTransaction>
{
    public override void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("PaymentTransaction");

        builder.Property(o => o.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Currency)
            .IsRequired()
            .HasColumnType("varchar(10)");

        builder.Property(o => o.Description)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(o => o.PaymentMethod)
            .IsRequired(false)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.PaymentType)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(o => o.TransactionCode)
            .IsRequired(false)
            .HasColumnType("varchar(50)");

        builder.Property(o => o.BankCode)
            .IsRequired(false)
            .HasColumnType("varchar(20)");

        builder.Property(o => o.BankName)
            .IsRequired(false)
            .HasColumnType("varchar(150)");

        builder.Property(o => o.OrderId);

    }
}
