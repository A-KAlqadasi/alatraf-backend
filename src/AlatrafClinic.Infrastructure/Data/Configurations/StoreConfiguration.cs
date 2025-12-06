using AlatrafClinic.Domain.Inventory.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(255);

        // Audit properties
        builder.Property(x => x.CreatedAtUtc)
               .IsRequired();

        builder.Property(x => x.CreatedBy)
               .HasMaxLength(200);

        builder.Property(x => x.LastModifiedUtc);
        builder.Property(x => x.LastModifiedBy)
               .HasMaxLength(200);

        // Owned collection: StoreItemUnits
        builder.OwnsMany(x => x.StoreItemUnits, siu =>
        {
            siu.ToTable("StoreItemUnits");
            siu.WithOwner().HasForeignKey("StoreId");

            siu.HasKey(s => s.Id);
            siu.Property(s => s.Id).ValueGeneratedOnAdd();

            siu.Property(s => s.StoreId).IsRequired();
            siu.Property(s => s.ItemUnitId).IsRequired();

            siu.Property(s => s.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

            siu.Property(s => s.CreatedAtUtc).IsRequired();
            siu.Property(s => s.CreatedBy).HasMaxLength(200);
            siu.Property(s => s.LastModifiedUtc);
            siu.Property(s => s.LastModifiedBy).HasMaxLength(200);

            // Foreign key to ItemUnit (external reference, not owned)
            siu.HasOne(s => s.ItemUnit)
               .WithMany()
               .HasForeignKey(s => s.ItemUnitId)
               .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Navigation(x => x.StoreItemUnits).AutoInclude(false);
    }
}
