using AlatrafClinic.Domain.Sales.SalesItems;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("SaleItemId");

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.StoreItemUnitId)
            .IsRequired(false);

        // FK to Sale
        builder.HasOne(i => i.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK to StoreItemUnit (Inventory traceability)
        builder.HasOne(i => i.StoreItemUnit)
            .WithMany(siu => siu.SaleItems)
            .HasForeignKey(i => i.StoreItemUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK to ItemUnit
        builder.HasOne(i => i.ItemUnit)
            .WithMany()
            .HasForeignKey(i => i.ItemUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.SaleId);

        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
