using AlatrafClinic.Domain.Inventory.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.SupplierName)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Phone)
               .IsRequired()
               .HasMaxLength(20);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);
    }
}
