using AlatrafClinic.Domain.Payments.DisabledPayments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DisabledPaymentConfiguration : IEntityTypeConfiguration<DisabledPayment>
{
    public void Configure(EntityTypeBuilder<DisabledPayment> builder)
    {
        builder.ToTable("DisabledPayments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id).ValueGeneratedNever()
            .HasColumnName("DisabledPaymentId");
        
        builder.Property(d => d.Notes)
            .HasColumnType("nvarchar")
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(d => d.Payment)
            .WithOne(p => p.DisabledPayment)
            .HasForeignKey<DisabledPayment>(d => d.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.DisabledCard)
            .WithMany(dc => dc.DisabledPayments)
            .HasForeignKey(d => d.DisabledCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
