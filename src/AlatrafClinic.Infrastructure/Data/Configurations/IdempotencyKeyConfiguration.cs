using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Idempotency;

public sealed class IdempotencyKeyConfiguration
    : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.ToTable("IdempotencyKeys");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Route)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.RequestHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => new { x.Key, x.Route }).IsUnique();
    }
}
