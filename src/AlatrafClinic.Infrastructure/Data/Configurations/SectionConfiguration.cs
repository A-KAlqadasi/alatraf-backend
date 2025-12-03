using AlatrafClinic.Domain.Departments.Sections;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("SectionId");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Department 1 -> many Sections (if Section has DepartmentId)
        builder.Property<int>("DepartmentId");

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Sections) // or .WithMany() if you don't have nav yet
            .HasForeignKey("DepartmentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.Name, s.DepartmentId });

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
