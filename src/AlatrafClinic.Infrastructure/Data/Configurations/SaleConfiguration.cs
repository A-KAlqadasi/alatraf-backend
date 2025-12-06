using AlatrafClinic.Domain.ExitCards;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Sales;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("SaleId");

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Relationship to SaleItems (if you add nav):
        builder.HasMany(s => s.SaleItems)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Diagnosis)
            .WithOne(d => d.Sale)
            .HasForeignKey<Sale>(s => s.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // builder.HasOne(s => s.ExchangeOrder)
        //     .WithOne(e => e.Sale)
        //     .HasForeignKey<ExchangeOrder>(e => e.SaleId)
        //     .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ExitCard)
            .WithOne(e => e.Sale)
            .HasForeignKey<ExitCard>(e => e.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.DiagnosisId)
            .IsUnique();

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
