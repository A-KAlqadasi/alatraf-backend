using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ExchangeOrderConfiguration : IEntityTypeConfiguration<ExchangeOrder>
{
    public void Configure(EntityTypeBuilder<ExchangeOrder> builder)
    {
        builder.ToTable("ExchangeOrders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Number)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.IsApproved)
               .IsRequired();

        builder.Property(x => x.Notes)
               .HasMaxLength(500);

        builder.Property(x => x.StoreId)
               .IsRequired();

        builder.Property(x => x.RelatedOrderId);
        builder.Property(x => x.RelatedSaleId);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);

        // Foreign key to Store
        builder.HasOne(x => x.Store)
               .WithMany()
               .HasForeignKey(x => x.StoreId)
               .OnDelete(DeleteBehavior.Restrict);

        // Owned collection: Items
        builder.OwnsMany(x => x.Items, items =>
        {
            items.ToTable("ExchangeOrderItems");
            items.WithOwner().HasForeignKey("ExchangeOrderId");

            items.HasKey(i => i.Id);
            items.Property(i => i.Id).ValueGeneratedOnAdd();

            items.Property(i => i.ExchangeOrderId).IsRequired();
            items.Property(i => i.StoreItemUnitId).IsRequired();
            items.Property(i => i.Quantity)
                 .IsRequired()
                 .HasPrecision(18, 3);

            items.Property(i => i.CreatedAtUtc).IsRequired();
            items.Property(i => i.CreatedBy).HasMaxLength(200);
            items.Property(i => i.LastModifiedUtc);
            items.Property(i => i.LastModifiedBy).HasMaxLength(200);
        });

        builder.Navigation(x => x.Items).AutoInclude(false);
    }
}
