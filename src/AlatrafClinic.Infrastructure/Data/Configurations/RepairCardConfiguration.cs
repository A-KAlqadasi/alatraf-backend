using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.DeliveryTimes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class RepairCardConfiguration : IEntityTypeConfiguration<RepairCard>
{
    public void Configure(EntityTypeBuilder<RepairCard> builder)
    {
        builder.ToTable("RepairCards");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("RepairCardId");

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.Property(r => r.IsActive)
            .IsRequired();

        builder.Property(r => r.Notes)
            .IsRequired(false)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);
        
        builder.HasOne(r => r.Diagnosis)
            .WithOne(d => d.RepairCard)
            .HasForeignKey<RepairCard>(r => r.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(r => r.Orders)
            .WithOne(o => o.RepairCard)
            .HasForeignKey(o => o.RepairCardId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(r => r.DiagnosisIndustrialParts)
            .WithOne(p => p.RepairCard)
            .HasForeignKey(p => p.RepairCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.DeliveryTime)
            .WithOne(d => d.RepairCard)
            .HasForeignKey<DeliveryTime>(d => d.RepairCardId);
        
        builder.HasIndex(r => r.DiagnosisId)
            .IsUnique();

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
