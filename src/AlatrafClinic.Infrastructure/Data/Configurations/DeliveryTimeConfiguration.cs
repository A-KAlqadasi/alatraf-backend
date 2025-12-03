using AlatrafClinic.Domain.RepairCards.DeliveryTimes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DeliveryTimeConfiguration : IEntityTypeConfiguration<DeliveryTime>
{
    public void Configure(EntityTypeBuilder<DeliveryTime> builder)
    {
        builder.ToTable("DeliveryTimes");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("DeliveryTimeId");

        builder.Property(d => d.DeliveryDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(d => d.Note)
            .IsRequired(false)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);

        builder.HasOne(d => d.RepairCard)
            .WithOne(rc => rc.DeliveryTime)
            .HasForeignKey<DeliveryTime>(d => d.RepairCardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
