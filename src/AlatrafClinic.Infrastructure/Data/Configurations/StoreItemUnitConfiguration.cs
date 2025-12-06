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

        builder.Property(x => x.StoreId)
               .IsRequired();

        builder.Property(x => x.ItemUnitId)
               .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);

        // Foreign key to ItemUnit
        builder.HasOne(x => x.ItemUnit)
               .WithMany()
               .HasForeignKey(x => x.ItemUnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
