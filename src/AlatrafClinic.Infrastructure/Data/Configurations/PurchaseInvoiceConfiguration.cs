using AlatrafClinic.Domain.Inventory.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("PurchaseInvoices");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Number)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Date)
               .IsRequired();

        builder.Property(x => x.SupplierId)
               .IsRequired();

        builder.Property(x => x.StoreId)
               .IsRequired();

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(x => x.PostedAtUtc);
        builder.Property(x => x.PaidAtUtc);

        builder.Property(x => x.PaymentAmount)
               .HasPrecision(18, 3);

        builder.Property(x => x.PaymentMethod)
               .HasMaxLength(100);

        builder.Property(x => x.PaymentReference)
               .HasMaxLength(100);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);

        // Foreign keys
        builder.HasOne(x => x.Supplier)
               .WithMany()
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Store)
               .WithMany()
               .HasForeignKey(x => x.StoreId)
               .OnDelete(DeleteBehavior.Restrict);

        // Owned collection: PurchaseItems
        builder.OwnsMany(x => x.Items, pi =>
        {
            pi.ToTable("PurchaseItems");
            pi.WithOwner().HasForeignKey("PurchaseInvoiceId");

            pi.HasKey(i => i.Id);
            pi.Property(i => i.Id).ValueGeneratedOnAdd();

            pi.Property(i => i.PurchaseInvoiceId).IsRequired();
            pi.Property(i => i.StoreItemUnitId).IsRequired();

            pi.Property(i => i.Quantity)
              .IsRequired()
              .HasPrecision(18, 3);

            pi.Property(i => i.UnitPrice)
              .IsRequired()
              .HasPrecision(18, 3);

            pi.Property(i => i.Notes)
              .HasMaxLength(500);

            pi.Property(i => i.CreatedAtUtc).IsRequired();
            pi.Property(i => i.CreatedBy).HasMaxLength(200);
            pi.Property(i => i.LastModifiedUtc);
            pi.Property(i => i.LastModifiedBy).HasMaxLength(200);

            // Foreign key to StoreItemUnit (external reference, not owned)
            pi.HasOne(i => i.StoreItemUnit)
               .WithMany()
               .HasForeignKey(i => i.StoreItemUnitId)
               .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Navigation(x => x.Items).AutoInclude(false);
    }
}
