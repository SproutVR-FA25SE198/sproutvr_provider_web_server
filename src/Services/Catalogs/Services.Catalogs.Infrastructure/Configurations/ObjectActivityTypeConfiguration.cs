using Common.Domain.Entities;
using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;

namespace Services.Catalogs.Infrastructure.Configurations;
public class ObjectActivityTypeConfiguration : BaseEntityConfiguration<ObjectActivityType>
{
    public override void Configure(EntityTypeBuilder<ObjectActivityType> builder)
    {
        builder.Ignore(nameof(BaseEntity.Id));
        
        base.Configure(builder);

        builder.ToTable("ObjectActivityType");

        // Composite key
        builder.HasKey(oat => new { oat.ActivityTypeId, oat.MapObjectId });

        builder.Property(oat => oat.MapObjectId)
            .IsRequired();

        builder.Property(oat => oat.ActivityTypeId)
            .IsRequired();

        builder.Ignore(oat => oat.UseIdKey);

        builder.HasOne(oat => oat.ActivityType)
                .WithMany()
                .HasForeignKey(oat => oat.ActivityTypeId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oat => oat.MapObject)
                .WithMany(mo => mo.ObjectActivityTypes)
                .HasForeignKey(oat => oat.MapObjectId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
