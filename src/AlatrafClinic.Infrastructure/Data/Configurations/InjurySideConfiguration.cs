using AlatrafClinic.Domain.Diagnosises.InjurySides;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;


public sealed class InjurySideConfiguration : IEntityTypeConfiguration<InjurySide>
{
    public void Configure(EntityTypeBuilder<InjurySide> builder)
    {
        builder.ToTable("InjurySides");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("InjurySideId");

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
