using Common.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.MasterSubjects;

namespace Services.Catalogs.Infrastructure.Configurations;
public class MasterSubjectConfiguration : BaseEntityConfiguration<MasterSubject>
{
    public override void Configure(EntityTypeBuilder<MasterSubject> builder)
    {
        base.Configure(builder);

        builder.ToTable("MasterSubject");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasColumnType("varchar(1000)")
            .HasMaxLength(1000);

        builder.Property(s => s.ImageUrl)
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);

        builder.Property(s => s.Status)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<MasterSubjectStatus>(s))
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        // Relationships
        builder.HasMany(ms => ms.Subjects)
                   .WithOne(s => s.MasterSubject)
                   .HasForeignKey(s => s.MasterSubjectId);
    }
}
