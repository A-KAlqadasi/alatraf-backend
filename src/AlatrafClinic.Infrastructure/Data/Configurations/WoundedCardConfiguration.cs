using AlatrafClinic.Domain.WoundedCards;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class WoundedCardConfiguration : IEntityTypeConfiguration<WoundedCard>
{
    public void Configure(EntityTypeBuilder<WoundedCard> builder)
    {
        builder.ToTable("WoundedCards");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("WoundedCardId");

        builder.Property(w => w.CardNumber)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(20);

        builder.Property(w => w.CardImagePath)
            .HasColumnType("nvarchar")
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(w => w.Expiration)
            .IsRequired()
            .HasColumnType("date");

        // Patient
        builder.HasOne(w => w.Patient)
            .WithOne(p => p.WoundedCard)
            .HasForeignKey<WoundedCard>(w => w.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.CardNumber)
            .IsUnique();

        builder.HasIndex(w => w.PatientId);

        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}
