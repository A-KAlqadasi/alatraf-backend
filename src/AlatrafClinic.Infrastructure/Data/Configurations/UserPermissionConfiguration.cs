using AlatrafClinic.Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.ToTable("UserPermissionOverrides");
        builder.HasKey(up => new { up.UserId, up.PermissionId });

        builder.Property(up => up.Effect)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired(true);

        builder.HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId);

        builder.HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissionOverrides)
            .HasForeignKey(up => up.PermissionId);
    }
}