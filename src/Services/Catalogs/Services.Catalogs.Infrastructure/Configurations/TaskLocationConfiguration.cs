using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Infrastructure.Configurations;
public class TaskLocationConfiguration : BaseEntityConfiguration<TaskLocation>
{
    public override void Configure(EntityTypeBuilder<TaskLocation> builder)
    {
        base.Configure(builder);
        builder.ToTable("TaskLocation");

        builder.Property(tl => tl.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(tl => tl.ImageUrl)
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);

        builder.Property(tl => tl.LocationIndex)
            .IsRequired();

        // Relationships
        builder.HasOne(tl => tl.Map)
            .WithMany(m => m.TaskLocations)
            .HasForeignKey(tl => tl.MapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(tl => tl.ObjectLocations)
            .WithOne(ol => ol.TaskLocation)
            .HasForeignKey(ol => ol.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
