using Litrater.Domain.Users;
using Litrater.Infrastructure.Common.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Litrater.Infrastructure.Users.Configurations;

public class UserConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.KeycloakUserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.KeycloakUserId)
            .IsUnique();
    }
}