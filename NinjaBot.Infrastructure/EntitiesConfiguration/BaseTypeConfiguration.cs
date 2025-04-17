using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NinjaBot.Domain.Interfaces;

namespace NinjaBot.Infrastructure.EntitiesConfiguration;

public class BaseTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IBaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(b => b.CreatedAt);
        builder.Property(b => b.CreatedBy).IsRequired().HasMaxLength(150);
        builder.Property(b => b.UpdatedBy).IsRequired(false).HasMaxLength(150);
    }
}
