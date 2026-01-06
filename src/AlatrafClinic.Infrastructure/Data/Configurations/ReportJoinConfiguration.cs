using AlatrafClinic.Domain.Reports;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ReportJoinConfiguration : IEntityTypeConfiguration<ReportJoin>
{
    public void Configure(EntityTypeBuilder<ReportJoin> builder)
    {
        builder.ToTable("ReportJoins");

        builder.HasKey(rj => rj.Id);

        builder.Property(rj => rj.FromTable)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rj => rj.ToTable)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(rj => rj.JoinType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rj => rj.JoinCondition)
            .IsRequired()
            .HasMaxLength(500);
    }
}