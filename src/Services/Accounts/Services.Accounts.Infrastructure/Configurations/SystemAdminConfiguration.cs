using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Accounts.Domain.Entities.SystemAdmins;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Infrastructure.Configurations;
public class SystemAdminConfiguration : IEntityTypeConfiguration<SystemAdmin>
{
    public void Configure(EntityTypeBuilder<SystemAdmin> builder)
    {
        builder.ToTable("SystemAdmin").HasBaseType<ApplicationUser>();
        builder.Property(sa => sa.FullName)
               .IsRequired()
               .HasColumnType("VARCHAR(255)");

    }
}
