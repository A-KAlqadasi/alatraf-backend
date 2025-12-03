using AlatrafClinic.Domain.Departments.DoctorSectionRooms;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DoctorSectionRoomConfiguration : IEntityTypeConfiguration<DoctorSectionRoom>
{
    public void Configure(EntityTypeBuilder<DoctorSectionRoom> builder)
    {
        builder.ToTable("DoctorSectionRooms");

        builder.HasKey(dsr => dsr.Id);
        builder.Property(dsr => dsr.Id)
            .HasColumnName("DoctorSectionRoomId");

        builder.Property(dsr => dsr.AssignDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(dsr => dsr.IsActive)
            .IsRequired();

        builder.Property(dsr => dsr.EndDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(dsr => dsr.Notes)
            .HasMaxLength(1000);

        builder.HasOne(dsr => dsr.Doctor)
            .WithMany(d => d.Assignments)
            .HasForeignKey(dsr => dsr.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dsr => dsr.Section)
            .WithMany(s => s.DoctorAssignments)
            .HasForeignKey(dsr => dsr.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dsr => dsr.Room)
            .WithMany(r => r.DoctorAssignments)
            .HasForeignKey(dsr => dsr.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(dsr => dsr.DiagnosisIndustrialParts)
            .WithOne(dip => dip.DoctorSectionRoom)
            .HasForeignKey(dip => dip.DoctorSectionRoomId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(dsr => dsr.SessionPrograms)
            .WithOne(ts => ts.DoctorSectionRoom)
            .HasForeignKey(ts => ts.DoctorSectionRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dsr => new { dsr.DoctorId, dsr.IsActive });
    }
}