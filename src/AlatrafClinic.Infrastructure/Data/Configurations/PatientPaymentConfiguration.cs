using AlatrafClinic.Domain.Payments.PatientPayments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class PatientPaymentConfiguration : IEntityTypeConfiguration<PatientPayment>
{
    public void Configure(EntityTypeBuilder<PatientPayment> builder)
    {
        builder.ToTable("PatientPayments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever()
            .HasColumnName("PatientPaymentId");

        builder.Property(p => p.VoucherNumber)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(50);

        builder.Property(p => p.Notes)
            .HasColumnType("nvarchar")
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(p => p.Payment)
            .WithOne(pp => pp.PatientPayment)
            .HasForeignKey<PatientPayment>(p => p.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
