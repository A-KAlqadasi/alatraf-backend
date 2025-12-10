using AlatrafClinic.Domain.Inventory.Purchases;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("PurchaseItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.PurchaseInvoiceId).IsRequired();
        builder.Property(i => i.StoreItemUnitId).IsRequired();

        builder.Property(i => i.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

        builder.Property(i => i.UnitPrice)
               .IsRequired()
               .HasPrecision(18, 3);

        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.Property(i => i.CreatedAtUtc).IsRequired();
        builder.Property(i => i.CreatedBy).HasMaxLength(200);
        builder.Property(i => i.LastModifiedUtc);
        builder.Property(i => i.LastModifiedBy).HasMaxLength(200);

        // Optional FK to StoreItemUnit
        builder.HasOne(i => i.StoreItemUnit)
               .WithMany()
               .HasForeignKey(i => i.StoreItemUnitId)
               .OnDelete(DeleteBehavior.Restrict);

        // If `IsDeleted` is required, ensure default exists
        builder.Property(i => i.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);
    }
}
