using AlatrafClinic.Domain.DisabledCards;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DisabledCardConfiguration : IEntityTypeConfiguration<DisabledCard>
{
    public void Configure(EntityTypeBuilder<DisabledCard> builder)
    {
        builder.ToTable("DisabledCards");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("DisabledCardId");

        builder.Property(d => d.CardNumber)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(20);
            
        builder.Property(d=> d.DisabilityType)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(100);

        builder.Property(d => d.CardImagePath)
            .HasColumnType("nvarchar")
            .IsRequired(false)
            .HasMaxLength(500);

            
        builder.Property(d=> d.IssueDate)
            .IsRequired()
            .HasColumnType("date");

        builder.HasOne(d => d.Patient)
            .WithOne(p=> p.DisabledCard)
            .HasForeignKey<DisabledCard>(d => d.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.CardNumber)
            .IsUnique();

        builder.HasIndex(d => d.PatientId);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
