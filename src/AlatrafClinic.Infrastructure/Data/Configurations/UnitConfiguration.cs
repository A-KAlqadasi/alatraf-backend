using AlatrafClinic.Domain.Inventory.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasMany(u => u.ItemUnits)
            .WithOne(iu => iu.Unit)
            .HasForeignKey(iu => iu.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(u => u.IndustrialPartUnits)
            .WithOne(ipu => ipu.Unit)
            .HasForeignKey(ipu => ipu.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.Name)
            .IsUnique();
    }
}