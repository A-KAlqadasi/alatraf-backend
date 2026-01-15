using AlatrafClinic.Domain.IndustrialParts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class IndustrialPartConfiguration : IEntityTypeConfiguration<IndustrialPart>
{
    public void Configure(EntityTypeBuilder<IndustrialPart> builder)
    {
        builder.ToTable("IndustrialParts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("IndustrialPartId");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasColumnType("nvarchar")
            .HasMaxLength(200);
        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
