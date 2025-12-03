using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DiagnosisProgramConfiguration : IEntityTypeConfiguration<DiagnosisProgram>
{
    public void Configure(EntityTypeBuilder<DiagnosisProgram> builder)
    {
        builder.ToTable("DiagnosisPrograms");

        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id)
            .HasColumnName("DiagnosisProgramId");

        builder.Property(dp => dp.Duration)
            .IsRequired();

        builder.Property(dp => dp.Notes)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(dp => dp.Diagnosis)
            .WithMany(d => d.DiagnosisPrograms)
            .HasForeignKey(dp => dp.DiagnosisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dp => dp.MedicalProgram)
            .WithMany(mp => mp.DiagnosisPrograms) 
            .HasForeignKey(dp => dp.MedicalProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dp => dp.TherapyCard)
            .WithMany(tc => tc.DiagnosisPrograms)
            .HasForeignKey(dp => dp.TherapyCardId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(dp => new { dp.DiagnosisId, dp.MedicalProgramId })
            .IsUnique();
    }
}
