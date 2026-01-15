using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Orders;
using AlatrafClinic.Domain.Sales;

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

              // // Audit properties
              // builder.Property(x => x.CreatedAtUtc)
              //        .IsRequired();

              // builder.Property(x => x.CreatedBy)
              //        .HasMaxLength(200);

              // builder.Property(x => x.LastModifiedUtc);
              // builder.Property(x => x.LastModifiedBy)
              //        .HasMaxLength(200);


              builder.HasOne(x => x.Store)
                             .WithMany()
                             .HasForeignKey(x => x.StoreId)
                             .OnDelete(DeleteBehavior.Restrict);

                     builder.HasOne<Order>()
                            .WithMany()
                            .HasForeignKey(x => x.RelatedOrderId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.HasOne<Sale>()
                            .WithMany()
                            .HasForeignKey(x => x.RelatedSaleId)
                            .OnDelete(DeleteBehavior.Restrict);

              // علاقة One-to-Many مع ExchangeOrderItem
              builder.HasMany(x => x.Items)
                     .WithOne(i => i.ExchangeOrder)
                     .HasForeignKey(i => i.ExchangeOrderId)
                     .OnDelete(DeleteBehavior.Cascade);

              builder.Navigation(x => x.Items).AutoInclude(false);
       }
}
