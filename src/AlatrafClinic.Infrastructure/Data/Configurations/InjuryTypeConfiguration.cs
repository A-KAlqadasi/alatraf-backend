using AlatrafClinic.Domain.Diagnosises.InjuryTypes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class InjuryTypeConfiguration : IEntityTypeConfiguration<InjuryType>
{
    public void Configure(EntityTypeBuilder<InjuryType> builder)
    {
        builder.ToTable("InjuryTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("InjuryTypeId");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("nvarchar(150)");

        // Unique name
        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
