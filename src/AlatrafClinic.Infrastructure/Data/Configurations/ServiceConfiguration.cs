using AlatrafClinic.Domain.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever()
            .HasColumnName("ServiceId");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(s => s.Code)
            .HasMaxLength(50)
            .IsRequired(false);
        
        builder.HasOne(s => s.Department)
            .WithMany(d=>d.Services)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(s => s.Tickets)
            .WithOne(t => t.Service)
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

         builder.HasIndex(s => s.Name);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}