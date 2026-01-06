using AlatrafClinic.Domain.Reports;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ReportDomainConfiguration : IEntityTypeConfiguration<ReportDomain>
{
    public void Configure(EntityTypeBuilder<ReportDomain> builder)
    {
        builder.ToTable("ReportDomains");

        builder.HasKey(rd => rd.Id);

        builder.Property(rd => rd.Id)
            .ValueGeneratedNever();

        builder.Property(rd => rd.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.RootTable)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(rd => rd.Fields)
            .WithOne()
            .HasForeignKey(rf => rf.DomainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(rd => rd.Joins)
            .WithOne()
            .HasForeignKey(rj => rj.DomainId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
