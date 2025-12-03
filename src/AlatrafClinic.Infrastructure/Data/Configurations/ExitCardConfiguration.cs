using AlatrafClinic.Domain.ExitCards;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class ExitCardConfiguration : IEntityTypeConfiguration<ExitCard>
{
    public void Configure(EntityTypeBuilder<ExitCard> builder)
    {
        builder.ToTable("ExitCards");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ExitCardId");

        builder.Property(e => e.Note)
            .HasColumnType("nvarchar")
            .HasMaxLength(1000);

        // Patient
        builder.HasOne(e => e.Patient)
            .WithMany(p => p.ExitCards)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Sale)
            .WithOne(s => s.ExitCard)
            .HasForeignKey<ExitCard>(e => e.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RepairCard)
            .WithOne(r => r.ExitCard)
            .HasForeignKey<ExitCard>(e => e.RepairCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.SaleId);
        builder.HasIndex(e => e.RepairCardId);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
