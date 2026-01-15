using AlatrafClinic.Domain.Orders;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
       public void Configure(EntityTypeBuilder<Order> builder)
       {
              builder.ToTable("Orders");

              // Primary key
              builder.HasKey(x => x.Id);
              builder.Property(x => x.Id).ValueGeneratedOnAdd();

              // RepairCard (optional FK)
              builder.Property(x => x.RepairCardId);

              builder.HasOne(x => x.RepairCard)
                     .WithMany()
                     .HasForeignKey(x => x.RepairCardId)
                     .OnDelete(DeleteBehavior.Restrict);

              // Section (required FK)
              builder.Property(x => x.SectionId).IsRequired();

              builder.HasOne(x => x.Section)
                     .WithMany()
                     .HasForeignKey(x => x.SectionId)
                     .OnDelete(DeleteBehavior.Restrict);

              // OrderType enum
              builder.Property(x => x.OrderType)
                     .HasConversion<string>()
                     .IsRequired()
                     .HasMaxLength(50);

              // Status enum
              builder.Property(x => x.Status)
                     .HasConversion<string>()
                     .IsRequired()
                     .HasMaxLength(50);

              // Audit fields (dependent on AuditableEntity<int>)
              builder.Property(x => x.CreatedAtUtc).IsRequired();
              builder.Property(x => x.CreatedBy).HasMaxLength(200);
              builder.Property(x => x.LastModifiedBy).HasMaxLength(200);

              // Navigation (Order has many OrderItems)
              builder.HasMany(x => x.OrderItems)
                     .WithOne(i => i.Order)
                     .HasForeignKey(i => i.OrderId)
                     .OnDelete(DeleteBehavior.Restrict); // مهم لمنع multiple cascade paths


              // // Audit
              // builder.Property(x => x.CreatedAtUtc).IsRequired();
              // builder.Property(x => x.CreatedBy).HasMaxLength(200);
              // builder.Property(x => x.LastModifiedUtc);
              // builder.Property(x => x.LastModifiedBy).HasMaxLength(200);



              builder.Navigation(x => x.OrderItems).AutoInclude(false);
       }
}
