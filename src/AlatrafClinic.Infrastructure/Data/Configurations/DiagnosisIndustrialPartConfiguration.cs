using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DiagnosisIndustrialPartConfiguration : IEntityTypeConfiguration<DiagnosisIndustrialPart>
{
    public void Configure(EntityTypeBuilder<DiagnosisIndustrialPart> builder)
    {
        builder.ToTable("DiagnosisIndustrialParts");

        builder.HasKey(dip => dip.Id);
        builder.Property(dip => dip.Id)
            .HasColumnName("DiagnosisIndustrialPartId");

        builder.Property(dip => dip.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(dip => dip.Quantity)
            .IsRequired();
        
        builder.Property(dip => dip.DoctorAssignDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.HasOne(dip => dip.Diagnosis)
            .WithMany(d => d.DiagnosisIndustrialParts)
            .HasForeignKey(dip => dip.DiagnosisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dip => dip.IndustrialPartUnit)
            .WithMany()
            .HasForeignKey(dip => dip.IndustrialPartUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dip => dip.DoctorSectionRoom)
            .WithMany(dsr => dsr.DiagnosisIndustrialParts)
            .HasForeignKey(dip => dip.DoctorSectionRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dip => dip.RepairCard)
            .WithMany(r => r.DiagnosisIndustrialParts)
            .HasForeignKey(dip => dip.RepairCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dip => new { dip.DiagnosisId, dip.IndustrialPartUnitId })
            .IsUnique();
    }
}

