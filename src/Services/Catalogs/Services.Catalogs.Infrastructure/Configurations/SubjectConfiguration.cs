using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalogs.Domain.Entities.Subjects;

namespace Services.Catalogs.Infrastructure.Configurations;
public class SubjectConfiguration : BaseEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        base.Configure(builder);

        builder.ToTable("Subject");

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
                s => Enum.Parse<SubjectStatus>(s))
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(s => s.MasterSubject)
                   .WithMany(ms => ms.Subjects)
                   .HasForeignKey(s => s.MasterSubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(s => s.Maps)
                   .WithOne(m => m.Subject)
                   .HasForeignKey(m => m.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
    }
}
