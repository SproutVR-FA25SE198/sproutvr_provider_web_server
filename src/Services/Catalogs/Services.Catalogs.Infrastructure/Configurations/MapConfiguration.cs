using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Infrastructure.Configurations;
public class MapConfiguration : BaseEntityConfiguration<Map>
{
    public override void Configure(EntityTypeBuilder<Map> builder)
    {
        base.Configure(builder);

        builder.ToTable("Map");

        builder.Property(m => m.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(m => m.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.Description)
            .HasColumnType("varchar(1000)")
            .HasMaxLength(1000);

        builder.Property(m => m.ImageUrl)
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);

        builder.Property(m => m.PreviewUrl)
            .HasColumnType("varchar(300)");

        builder.Property(m => m.Status)
            .HasConversion(
                m => m.ToString(),
                m => Enum.Parse<MapStatus>(m))
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(m => m.MapCode)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(m => m.MetadataStoragePath)
            .IsRequired(false)
            .HasColumnType("varchar(500)")
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(m => m.Subject)
            .WithMany(s => s.Maps)
            .HasForeignKey(m => m.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.MapObjects)
            .WithOne(mo => mo.Map)
            .HasForeignKey(mo => mo.MapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.TaskLocations)
            .WithOne(tl => tl.Map)
            .HasForeignKey(tl => tl.MapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
