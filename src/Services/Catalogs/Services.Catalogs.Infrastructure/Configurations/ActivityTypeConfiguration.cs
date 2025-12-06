using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.ActivityTypes;

namespace Services.Catalogs.Infrastructure.Configurations;
public class ActivityTypeConfiguration : BaseEntityConfiguration<ActivityType>
{
    public override void Configure(EntityTypeBuilder<ActivityType> builder)
    {
        base.Configure(builder);

        builder.ToTable("ActivityType");

        builder.Property(at => at.ActivityCode)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(at => at.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);
   }
}
