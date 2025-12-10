using AlatrafClinic.Domain.Inventory.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
         builder.ToTable("Items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
               .IsRequired();

        builder.Property(x => x.BaseUnitId)
               .IsRequired();

        // // Audit
        // builder.Property(x => x.CreatedAtUtc).IsRequired();
        // builder.Property(x => x.CreatedBy).HasMaxLength(200);
        // builder.Property(x => x.LastModifiedUtc);
        // builder.Property(x => x.LastModifiedBy).HasMaxLength(200);

        // Base Unit relation
        builder.HasOne(x => x.BaseUnit)
               .WithMany()
               .HasForeignKey(x => x.BaseUnitId)
               .OnDelete(DeleteBehavior.Restrict);

        // ⛔️ تم استبدال الـ OwnedRelation بعلاقة عادية
        builder.HasMany(x => x.ItemUnits)
               .WithOne(iu => iu.Item)
               .HasForeignKey(iu => iu.ItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ItemUnits).AutoInclude(false); }
}
