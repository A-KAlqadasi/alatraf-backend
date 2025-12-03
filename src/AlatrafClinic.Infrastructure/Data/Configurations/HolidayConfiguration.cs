using AlatrafClinic.Domain.Services.Appointments.Holidays;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.ToTable("Holidays");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasColumnName("HolidayId");

        builder.Property(h => h.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(h => h.StartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(h => h.EndDate)
            .HasColumnType("date");

        builder.Property(h => h.IsRecurring)
            .IsRequired();

        builder.Property(h => h.IsActive)
            .IsRequired();

        builder.Property(h => h.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(h => new { h.StartDate, h.EndDate, h.IsRecurring, h.Type });
    }
}
