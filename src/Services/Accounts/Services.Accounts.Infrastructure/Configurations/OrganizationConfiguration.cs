using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Infrastructure.Configurations;
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // table per type
        builder.ToTable("Organization").HasBaseType<ApplicationUser>();

        builder.Property(o => o.MACAddress)
                .HasColumnType("VARCHAR(20)");

        builder.HasIndex(o => o.MACAddress)
                .IsUnique();

        builder.Property(o => o.Name)
                .IsRequired()
                .HasColumnType("VARCHAR(255)");

        builder.Property(o => o.RepresentativeName)
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
                .HasColumnType("VARCHAR(100)");

        builder.Property(o => o.ActivationKey)
                .HasColumnType("VARCHAR(255)");

        builder.Property(o => o.BundleGoogleDriveUrl)
                .HasColumnType("VARCHAR(255)");

        builder.HasIndex(o => o.ActivationKey).IsUnique();
        
        builder.HasIndex(o => o.ContactEmail).IsUnique();
        builder.HasIndex(o => o.ContactPhone).IsUnique();


    }
}
