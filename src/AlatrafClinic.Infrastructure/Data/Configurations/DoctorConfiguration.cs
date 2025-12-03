using AlatrafClinic.Domain.People.Doctors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("DoctorId");

        builder.Property(d => d.IsActive)
            .IsRequired();

        builder.Property(d => d.Specialization)
            .HasColumnType("nvarchar")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasOne(d => d.Person)
            .WithOne(p=> p.Doctor) 
            .HasForeignKey<Doctor>(p=> p.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d=> d.Department)
            .WithMany(department => department.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
         builder.HasMany(d => d.Assignments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p=> p.PersonId)
            .IsUnique();

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
