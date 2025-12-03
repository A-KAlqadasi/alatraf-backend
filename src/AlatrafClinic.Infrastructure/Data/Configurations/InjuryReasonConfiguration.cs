using AlatrafClinic.Domain.Diagnosises.InjuryReasons;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class InjuryReasonConfiguration : IEntityTypeConfiguration<InjuryReason>
{
    public void Configure(EntityTypeBuilder<InjuryReason> builder)
    {
        builder.ToTable("InjuryReasons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("InjuryReasonId");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(150);

        // Unique name
        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
