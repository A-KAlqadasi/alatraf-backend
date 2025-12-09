using AlatrafClinic.Domain.Patients;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("PatientId");

        builder.Property(x => x.PersonId)
            .IsRequired();

        builder.Property(x => x.PatientType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Person)
            .WithOne(x=> x.Patient)
            .HasForeignKey<Patient>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // One patient per person (business rule)
        builder.HasIndex(x => x.PersonId)
            .IsUnique();

        // Index on PatientType for filtering
        builder.HasIndex(x => x.PatientType);

        // Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
