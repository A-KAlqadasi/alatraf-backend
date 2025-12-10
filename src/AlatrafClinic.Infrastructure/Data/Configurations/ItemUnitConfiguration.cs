using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Units;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ItemUnitConfiguration : IEntityTypeConfiguration<ItemUnit>
{
    public void Configure(EntityTypeBuilder<ItemUnit> builder)
    {
        builder.ToTable("ItemUnits");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Foreign keys
        builder.Property(x => x.ItemId).IsRequired();
        builder.Property(x => x.UnitId).IsRequired();

        builder.HasOne(iu => iu.Item)
               .WithMany(i => i.ItemUnits)
               .HasForeignKey(iu => iu.ItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(iu => iu.Unit)
               .WithMany(u => u.ItemUnits)
               .HasForeignKey(iu => iu.UnitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Price)
               .IsRequired()
               .HasPrecision(18, 3);

        builder.Property(x => x.MinPriceToPay)
               .HasPrecision(18, 3);

        builder.Property(x => x.MaxPriceToPay)
               .HasPrecision(18, 3);

        builder.Property(x => x.ConversionFactor)
               .IsRequired()
               .HasDefaultValue(1)
               .HasPrecision(18, 3);

        // // Audit
        // builder.Property(x => x.CreatedAtUtc).IsRequired();
        // builder.Property(x => x.CreatedBy).HasMaxLength(200);
        // builder.Property(x => x.LastModifiedUtc);
        // builder.Property(x => x.LastModifiedBy).HasMaxLength(200);
    }
}
