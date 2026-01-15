using AlatrafClinic.Domain.MedicalPrograms;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class MedicalProgramsConfiguration : IEntityTypeConfiguration<MedicalProgram>
{
    public void Configure(EntityTypeBuilder<MedicalProgram> builder)
    {
        builder.ToTable("MedicalPrograms");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Id)
            .HasColumnName("MedicalProgramId");

        builder.Property(mp => mp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(mp => mp.Description)
            .IsRequired(false)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);

        builder.HasMany(mp => mp.DiagnosisPrograms)
            .WithOne(dp => dp.MedicalProgram)
            .HasForeignKey(dp => dp.MedicalProgramId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(mp => mp.Section)
            .WithMany(s => s.MedicalPrograms)
            .HasForeignKey(mp => mp.SectionId)
            .OnDelete(DeleteBehavior.Restrict);
        

        builder.HasIndex(mp => mp.Name)
            .IsUnique();

        builder.HasQueryFilter(mp => !mp.IsDeleted);
    }
}
