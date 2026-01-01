using AlatrafClinic.Infrastructure.Data.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class ProcessedMessageConfiguration
    : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("ProcessedMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HandlerName)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => new { x.MessageId, x.HandlerName })
            .IsUnique();
    }
}
