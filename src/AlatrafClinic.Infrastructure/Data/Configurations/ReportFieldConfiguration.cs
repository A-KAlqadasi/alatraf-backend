using AlatrafClinic.Domain.Reports;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public class ReportFieldConfiguration : IEntityTypeConfiguration<ReportField>
{
    public void Configure(EntityTypeBuilder<ReportField> builder)
    {
        builder.ToTable("ReportFields");

        builder.HasKey(rf => rf.Id);

        builder.Property(rf => rf.FieldKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rf => rf.DisplayName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(rf => rf.TableName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rf => rf.ColumnName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(rf => rf.DataType)
            .IsRequired()
            .HasMaxLength(100);
    }
}
