using Common.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Infrastructure.Configurations;
public class ObjectLocationConfiguration : BaseEntityConfiguration<ObjectLocation>
{
    public override void Configure(EntityTypeBuilder<ObjectLocation> builder)
    {
        base.Configure(builder);

        builder.ToTable("ObjectLocation");

        // Composite key for unique constraint
        builder.HasIndex(ol => new { ol.ObjectId, ol.LocationId }).IsUnique();

        builder.Property(ol => ol.ObjectId)
            .IsRequired();

        builder.Property(ol => ol.LocationId)
            .IsRequired();

        builder.HasOne(ol => ol.TaskLocation)
                .WithMany(tl => tl.ObjectLocations)
                .HasForeignKey(ol => ol.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ol => ol.MapObject)
                .WithMany()
                .HasForeignKey(ol => ol.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
