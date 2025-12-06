using AlatrafClinic.Domain.RepairCards.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.RepairCardId);

        builder.Property(x => x.SectionId)
               .IsRequired();

        builder.Property(x => x.OrderType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        // Audit
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(200);
        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(200);

        // Owned collection: OrderItems
        builder.OwnsMany(x => x.OrderItems, oi =>
        {
            oi.ToTable("OrderItems");
            oi.WithOwner().HasForeignKey("OrderId");

            oi.HasKey(i => i.Id);
            oi.Property(i => i.Id).ValueGeneratedOnAdd();

            oi.Property(i => i.OrderId).IsRequired();
            oi.Property(i => i.ItemUnitId).IsRequired();

            oi.Property(i => i.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

            oi.Property(i => i.CreatedAtUtc).IsRequired();
            oi.Property(i => i.CreatedBy).HasMaxLength(200);
            oi.Property(i => i.LastModifiedUtc);
            oi.Property(i => i.LastModifiedBy).HasMaxLength(200);
        });

        builder.Navigation(x => x.OrderItems).AutoInclude(false);
    }
}
