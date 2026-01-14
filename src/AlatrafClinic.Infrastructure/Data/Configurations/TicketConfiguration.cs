using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Tickets;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("TicketId");

        builder.Property(t => t.ServicePrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasOne(t => t.Patient)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Service)
            .WithMany(s=> s.Tickets)
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Diagnosis)
            .WithOne(d => d.Ticket)
            .HasForeignKey<Diagnosis>(d => d.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Appointment)
            .WithOne(a => a.Ticket)
            .HasForeignKey<Appointment>(a => a.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        // useful indexes for queries
        builder.HasIndex(t => t.PatientId);
        builder.HasIndex(t => t.ServiceId);
        builder.HasIndex(t => t.Status);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
