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

              // // Audit properties
              // builder.Property(x => x.CreatedAtUtc)
              //        .IsRequired();

              // builder.Property(x => x.CreatedBy)
              //        .HasMaxLength(200);

              // builder.Property(x => x.LastModifiedUtc);
              // builder.Property(x => x.LastModifiedBy)
              //        .HasMaxLength(200);

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
              // 🔥 Change from Owned to Dependent Entity
              builder.HasMany(x => x.Items)
                     .WithOne(i => i.PurchaseInvoice)
                     .HasForeignKey(i => i.PurchaseInvoiceId)
                     .OnDelete(DeleteBehavior.Cascade);

              builder.Navigation(x => x.Items).AutoInclude(false);
       }
}
