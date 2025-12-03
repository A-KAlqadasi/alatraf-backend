using AlatrafClinic.Domain.Payments.WoundedPayments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class WoundedPaymentConfiguration : IEntityTypeConfiguration<WoundedPayment>
{
    public void Configure(EntityTypeBuilder<WoundedPayment> builder)
    {
        builder.ToTable("WoundedPayments");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasColumnName("WoundedPaymentId");

        builder.HasOne(w => w.Payment)
            .WithOne(p=> p.WoundedPayment)
            .HasForeignKey<WoundedPayment>(w => w.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.WoundedCard)
            .WithMany(wc => wc.WoundedPayments)
            .HasForeignKey(w => w.WoundedCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}
