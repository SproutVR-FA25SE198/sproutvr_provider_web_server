using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Data.Configurations;
public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {

        T instance = Activator.CreateInstance<T>();
        if (instance.UseIdKey)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }

        builder.Property(e => e.CreatedAtUtc)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.UpdatedAtUtc)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
