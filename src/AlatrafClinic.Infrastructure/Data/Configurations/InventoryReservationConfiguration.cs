using AlatrafClinic.Domain.Inventory.Reservations;
using AlatrafClinic.Domain.Inventory.Stores;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations.Inventory;

public class InventoryReservationConfiguration
    : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.ToTable("InventoryReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever(); // Guid يتم توليده في الدومين

        builder.Property(x => x.SaleId)
               .IsRequired();

        builder.Property(x => x.SagaId)
               .IsRequired(false);

        builder.Property(x => x.StoreItemUnitId).HasColumnName("StoreItemUnitId")
               .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired()
               .HasPrecision(18, 3);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<int>();

        // علاقة مع StoreItemUnit
        builder.HasOne(x => x.StoreItemUnit)
       .WithMany()
       .HasForeignKey(x => x.StoreItemUnitId)
       .OnDelete(DeleteBehavior.Restrict);

        // Index مهم للأداء (Saga / Lookups)
        // Preserve existing uniqueness on Sale + StoreItemUnit to avoid breaking schema
        builder.HasIndex(x => new { x.SaleId, x.StoreItemUnitId })
               .IsUnique();

        // Optional index to speed up saga lookups without enforcing uniqueness
        builder.HasIndex(x => new { x.SagaId, x.SaleId });
    }
}
