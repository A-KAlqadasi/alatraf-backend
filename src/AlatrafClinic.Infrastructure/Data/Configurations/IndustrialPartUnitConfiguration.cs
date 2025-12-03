using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class IndustrialPartUnitConfiguration : IEntityTypeConfiguration<IndustrialPartUnit>
{
    public void Configure(EntityTypeBuilder<IndustrialPartUnit> builder)
    {
        builder.ToTable("IndustrialPartUnits");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("IndustrialPartUnitId");

        builder.Property(u => u.PricePerUnit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.HasOne(idpu => idpu.IndustrialPart)
            .WithMany(ip => ip.IndustrialPartUnits)
            .HasForeignKey(idpu => idpu.IndustrialPartId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(idpu => idpu.Unit)
            .WithMany(u=> u.IndustrialPartUnits)
            .HasForeignKey(idpu => idpu.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
