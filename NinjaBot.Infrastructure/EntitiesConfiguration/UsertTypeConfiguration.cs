using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NinjaBot.Domain.Entities;


namespace NinjaBot.Infrastructure.EntitiesConfiguration;

public class UsertTypeConfiguration : BaseTypeConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Password).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(50);
        builder.Property(x => x.Universe).HasMaxLength(20);
        builder.Property(x => x.Language).HasMaxLength(10);
    }
}
