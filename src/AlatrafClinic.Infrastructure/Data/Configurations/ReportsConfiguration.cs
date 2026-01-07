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
            .ValueGeneratedOnAdd(); // Or change to ValueGeneratedOnAdd() if you want DB to generate

        builder.Property(rd => rd.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.RootTable)
            .IsRequired()
            .HasMaxLength(100);
            
        // New properties
        builder.Property(rd => rd.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(rd => rd.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()"); // Use GETUTCDATE() for UTC
            
        builder.Property(rd => rd.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate(); // Auto-update on save

        builder.HasMany(rd => rd.Fields)
            .WithOne(rf => rf.Domain)
            .HasForeignKey(rf => rf.DomainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(rd => rd.Joins)
            .WithOne(rj => rj.Domain)
            .HasForeignKey(rj => rj.DomainId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index for performance
        builder.HasIndex(rd => rd.IsActive);

        builder.HasQueryFilter(rd => rd.IsActive); // Global filter to only get active domains
    }
}

public class ReportFieldConfiguration : IEntityTypeConfiguration<ReportField>
{
    public void Configure(EntityTypeBuilder<ReportField> builder)
    {
        builder.ToTable("ReportFields");

        builder.HasKey(rf => rf.Id);
        
        builder.Property(rf => rf.Id)
            .ValueGeneratedOnAdd(); // Auto-increment

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
            
        // New properties
        builder.Property(rf => rf.IsFilterable)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(rf => rf.IsSortable)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(rf => rf.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(rf => rf.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(rf => rf.DefaultOrder)
            .IsRequired(false);
            
        builder.Property(rf => rf.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(rf => rf.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

        // Foreign key relationship
        builder.HasOne(rf => rf.Domain)
            .WithMany(rd => rd.Fields)
            .HasForeignKey(rf => rf.DomainId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes for performance
        builder.HasIndex(rf => rf.DomainId);
        builder.HasIndex(rf => rf.IsActive);
        builder.HasQueryFilter(rf => rf.IsActive); // Global filter to only get active fields
        builder.HasIndex(rf => rf.FieldKey)
            .IncludeProperties(rf => new { rf.DisplayName, rf.TableName, rf.ColumnName });
    }
}

public class ReportJoinConfiguration : IEntityTypeConfiguration<ReportJoin>
{
    public void Configure(EntityTypeBuilder<ReportJoin> builder)
    {
        builder.ToTable("ReportJoins");

        builder.HasKey(rj => rj.Id);
        
        builder.Property(rj => rj.Id)
            .ValueGeneratedOnAdd();

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
            
        // New properties
        builder.Property(rj => rj.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(rj => rj.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(rj => rj.JoinOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(rj => rj.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(rj => rj.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

        // Foreign key relationship
        builder.HasOne(rj => rj.Domain)
            .WithMany(rd => rd.Joins)
            .HasForeignKey(rj => rj.DomainId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes for performance
        builder.HasIndex(rj => rj.DomainId);
        builder.HasIndex(rj => rj.IsActive);
        builder.HasQueryFilter(rj => rj.IsActive); // Global filter to only get active joins
        builder.HasIndex(rj => new { rj.FromTable, rj.ToTable });
    }
}