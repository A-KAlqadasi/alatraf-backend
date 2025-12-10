using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ExchangeOrderItemConfiguration : IEntityTypeConfiguration<ExchangeOrderItem>
{
    public void Configure(EntityTypeBuilder<ExchangeOrderItem> builder)
    {
        builder.ToTable("ExchangeOrderItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ExchangeOrderId).IsRequired();
        builder.Property(x => x.StoreItemUnitId).IsRequired();
        builder.Property(x => x.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(200);
        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(200);

        // FK relation (optional, تكملة السلامة)
        builder.HasOne(x => x.ExchangeOrder)
               .WithMany(o => o.Items)
               .HasForeignKey(x => x.ExchangeOrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
