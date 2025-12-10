using AlatrafClinic.Domain.Inventory.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class StoreItemUnitConfiguration : IEntityTypeConfiguration<StoreItemUnit>
{
    public void Configure(EntityTypeBuilder<StoreItemUnit> builder)
    {
         builder.ToTable("StoreItemUnits");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.StoreId).IsRequired();
        builder.Property(x => x.ItemUnitId).IsRequired();
        builder.Property(x => x.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

        // علاقة مع Store
        builder.HasOne(x => x.Store)
               .WithMany(s => s.StoreItemUnits)
               .HasForeignKey(x => x.StoreId)
               .OnDelete(DeleteBehavior.Cascade);

        // علاقة مع ItemUnit
        builder.HasOne(x => x.ItemUnit)
               .WithMany()
               .HasForeignKey(x => x.ItemUnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
