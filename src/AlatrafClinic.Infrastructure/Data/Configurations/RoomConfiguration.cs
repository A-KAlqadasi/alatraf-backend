using AlatrafClinic.Domain.Departments.Sections.Rooms;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("RoomId");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(r => r.Section)
            .WithMany(s => s.Rooms)
            .HasForeignKey(r => r.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.SectionId, r.Name })
            .IsUnique();

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
