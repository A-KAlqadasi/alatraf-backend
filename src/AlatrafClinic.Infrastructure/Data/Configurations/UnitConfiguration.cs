using AlatrafClinic.Domain.Inventory.Units;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<GeneralUnit>
{
       public void Configure(EntityTypeBuilder<GeneralUnit> builder)
       {
              builder.ToTable("Units");

              builder.HasKey(x => x.Id);
              builder.Property(x => x.Id).ValueGeneratedOnAdd();

              builder.Property(x => x.Name)
                     .IsRequired()
                     .HasMaxLength(100);

              // // Audit properties
              // builder.Property(x => x.CreatedAtUtc)
              //        .IsRequired();

              // builder.Property(x => x.CreatedBy)
              //        .HasMaxLength(200);

              // builder.Property(x => x.LastModifiedUtc);
              // builder.Property(x => x.LastModifiedBy)
              //        .HasMaxLength(200);

              // Navigation to ItemUnits (not owned; many-to-many relationship)
              // Unit does not own ItemUnits; Item owns them via OwnsMany
              builder.HasMany(x => x.ItemUnits)
                     .WithOne(iu => iu.Unit)
                     .HasForeignKey(iu => iu.UnitId)
                     .OnDelete(DeleteBehavior.Restrict);

              // Navigation to IndustrialPartUnits
              builder.HasMany(x => x.IndustrialPartUnits)
                     .WithOne(ipu => ipu.Unit)
                     .HasForeignKey(ipu => ipu.UnitId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
