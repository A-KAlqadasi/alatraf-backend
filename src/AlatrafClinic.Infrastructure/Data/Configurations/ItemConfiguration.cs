using AlatrafClinic.Domain.Inventory.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
               .IsRequired();

        builder.Property(x => x.BaseUnitId)
               .IsRequired();

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);

        // Foreign key to Unit (base unit)
        builder.HasOne(x => x.BaseUnit)
               .WithMany()
               .HasForeignKey(x => x.BaseUnitId)
               .OnDelete(DeleteBehavior.Restrict);

        // Owned collection: ItemUnits
        builder.OwnsMany(x => x.ItemUnits, iu =>
        {
            iu.ToTable("ItemUnits");
            iu.WithOwner().HasForeignKey("ItemId");

            iu.HasKey(i => i.Id);
            iu.Property(i => i.Id).ValueGeneratedOnAdd();

            iu.Property(i => i.ItemId).IsRequired();
            iu.Property(i => i.UnitId).IsRequired();

            iu.Property(i => i.Price)
              .IsRequired()
              .HasPrecision(18, 3);

            iu.Property(i => i.MinPriceToPay)
              .HasPrecision(18, 3);

            iu.Property(i => i.MaxPriceToPay)
              .HasPrecision(18, 3);

            iu.Property(i => i.ConversionFactor)
              .IsRequired()
              .HasDefaultValue(1)
              .HasPrecision(18, 3);

            iu.Property(i => i.CreatedAtUtc).IsRequired();
            iu.Property(i => i.CreatedBy).HasMaxLength(200);
            iu.Property(i => i.LastModifiedUtc);
            iu.Property(i => i.LastModifiedBy).HasMaxLength(200);
        });

        builder.Navigation(x => x.ItemUnits).AutoInclude(false);
    }
}
