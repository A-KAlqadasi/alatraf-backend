using AlatrafClinic.Domain.TherapyCards;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class TherapyCardConfiguration : IEntityTypeConfiguration<TherapyCard>
{
    public void Configure(EntityTypeBuilder<TherapyCard> builder)
    {
        builder.ToTable("TherapyCards");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Id)
            .HasColumnName("TherapyCardId");

        builder.Property(tc => tc.ProgramStartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(tc => tc.ProgramEndDate)
            .HasColumnType("date");
            
        builder.Property(tc => tc.NumberOfSessions)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(tc => tc.IsActive)
            .IsRequired();
        
        builder.Property(tc => tc.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(tc => tc.SessionPricePerType)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(tc => tc.CardStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasMany(tc => tc.DiagnosisPrograms)
            .WithOne(dp => dp.TherapyCard)
            .HasForeignKey(dp => dp.TherapyCardId);
        
        builder.HasOne(tc => tc.Diagnosis)
            .WithOne(d => d.TherapyCard)
            .HasForeignKey<TherapyCard>(tc => tc.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(tc => tc.Sessions)
            .WithOne(s => s.TherapyCard)
            .HasForeignKey(s => s.TherapyCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property<int?>(tc => tc.ParentCardId)
            .IsRequired(false);
        
        builder.HasQueryFilter(tc => !tc.IsDeleted);
    }
}

