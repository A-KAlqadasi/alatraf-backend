using AlatrafClinic.Domain.Diagnosises;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
{
    public void Configure(EntityTypeBuilder<Diagnosis> builder)
    {
        builder.ToTable("Diagnoses");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("DiagnosisId");

        builder.Property(d => d.DiagnosisText)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.InjuryDate)
            .HasColumnType("date");

        builder.Property(d => d.DiagnoType)
            .HasColumnName("DiagnosisType")
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(d => d.Ticket)
            .WithOne(t => t.Diagnosis)
            .HasForeignKey<Diagnosis>(d => d.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Patient)
            .WithMany(p => p.Diagnoses)
            .HasForeignKey(d => d.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(d=> d.InjuryReasons)
            .WithMany(injreason => injreason.Diagnoses);
        
        builder.HasMany(d=> d.InjurySides)
            .WithMany(injside => injside.Diagnoses);
        
        builder.HasMany(d=> d.InjuryTypes)
            .WithMany(injtype => injtype.Diagnoses);


        builder.HasIndex(d => d.TicketId)
            .IsUnique();

        builder.HasIndex(d => d.PatientId);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
