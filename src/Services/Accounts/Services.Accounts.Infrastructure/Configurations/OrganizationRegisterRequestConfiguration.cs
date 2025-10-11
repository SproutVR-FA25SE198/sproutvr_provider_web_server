using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Infrastructure.Configurations;
public class OrganizationRegisterRequestConfiguration :  BaseEntityConfiguration<OrganizationRegisterRequest>
{
    public override void Configure(EntityTypeBuilder<OrganizationRegisterRequest> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("OrganizationRegisterRequest");

        builder.Property(o => o.OrganizationName)
               .IsRequired()
               .HasColumnType("VARCHAR(255)");

        builder.Property(o => o.Address)
               .IsRequired()
               .HasColumnType("VARCHAR(500)");

        builder.Property(o => o.ContactPhone)
               .IsRequired()
               .HasColumnType("VARCHAR(20)");

        builder.Property(o => o.ContactEmail)
               .IsRequired()
               .HasColumnType("VARCHAR(255)");

        builder.Property(o => o.ApprovalStatus)
               .IsRequired()
               .HasConversion(
                   m => m.ToString(),
                   m => Enum.Parse<ApprovalStatus>(m))
               .HasColumnType("varchar(50)")
               .HasMaxLength(50)
               .HasDefaultValue(ApprovalStatus.Pending);
    }
}
