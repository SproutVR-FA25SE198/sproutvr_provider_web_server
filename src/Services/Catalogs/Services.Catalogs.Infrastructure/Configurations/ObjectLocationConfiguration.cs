using Common.Domain.Entities;
using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Infrastructure.Configurations;
public class ObjectLocationConfiguration : BaseEntityConfiguration<ObjectLocation>
{
    public override void Configure(EntityTypeBuilder<ObjectLocation> builder)
    {
        builder.Ignore(nameof(BaseEntity.Id));

        base.Configure(builder);

        builder.ToTable("ObjectLocation");

        // Composite key for unique constraint
        builder.HasKey(ol => new { ol.ObjectId, ol.TaskLocationId });

        builder.Property(ol => ol.ObjectId)
            .IsRequired();

        builder.Property(ol => ol.TaskLocationId)
            .IsRequired();

        builder.Ignore(ol => ol.UseIdKey);

        builder.HasOne(ol => ol.TaskLocation)
                .WithMany(tl => tl.ObjectLocations)
                .HasForeignKey(ol => ol.TaskLocationId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ol => ol.MapObject)
                .WithMany()
                .HasForeignKey(ol => ol.ObjectId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
