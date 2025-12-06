using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.HasIndex(u => u.PhoneNumber).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion(
                m => m.ToString(),
                m => Enum.Parse<AccountStatus>(m))
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .HasDefaultValue(AccountStatus.Active);

        builder.Property(u => u.AvatarUrl)
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        builder.Property(u => u.CreatedAtUtc)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.UpdatedAtUtc)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
