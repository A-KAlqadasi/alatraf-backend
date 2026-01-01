using AlatrafClinic.Domain.Payments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("PaymentId");

        builder.Property(p => p.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PaidAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(p => p.Discount)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(p => p.PaymentDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(p => p.IsCompleted)
            .IsRequired(true);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.Property(p => p.AccountKind)
            .IsRequired(false)
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(p => p.PaymentReference)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(p => p.SagaId)
            .IsRequired(false);

        builder.HasOne(p => p.Diagnosis)
            .WithMany(d => d.Payments)
            .HasForeignKey(p => p.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Ticket)
            .WithOne(t => t.Payment)
            .HasForeignKey<Payment>(p => p.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => new { p.SagaId, p.PaymentReference, p.DiagnosisId }).IsUnique(false);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
