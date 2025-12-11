using AlatrafClinic.Domain.Services.Appointments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("AppointmentId");

        builder.Property(a => a.AttendDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(a => a.PatientType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Notes)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(a => a.Ticket)
            .WithOne(t => t.Appointment)
            .HasForeignKey<Appointment>(a => a.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.TicketId)
            .IsUnique();

        builder.HasIndex(a => a.AttendDate);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.PatientType);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
