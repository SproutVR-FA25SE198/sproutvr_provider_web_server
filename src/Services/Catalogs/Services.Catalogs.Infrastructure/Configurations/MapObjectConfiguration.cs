using Common.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.MapObjects;

namespace Services.Catalogs.Infrastructure.Configurations;
public class MapObjectConfiguration : BaseEntityConfiguration<MapObject>
{
    public override void Configure(EntityTypeBuilder<MapObject> builder)
    {
        base.Configure(builder);
        builder.ToTable("MapObject");

        builder.Property(mo => mo.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);
        builder.Property(mo => mo.ObjectCode)
           .IsRequired()
           .HasColumnType("varchar(100)")
           .HasMaxLength(100);

        builder.Property(mo => mo.ImageUrl)
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);
        
        // Relationships
        builder.HasOne(mo => mo.Map)
            .WithMany(m => m.MapObjects)
            .HasForeignKey(mo => mo.MapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(mo => mo.ObjectActivityTypes)
            .WithOne(oat => oat.MapObject)
            .HasForeignKey(oat => oat.MapObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // constraints
        builder.HasIndex(mo => new { mo.ObjectCode, mo.MapId })
            .IsUnique();
    }
}
