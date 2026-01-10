using AlatrafClinic.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("PersonId");

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasColumnType("nvarchar(100)");

        builder.Property(x => x.Birthdate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasColumnType("nvarchar(15)");

        builder.Property(x => x.NationalNo)
            .HasColumnType("nvarchar(20)");

        builder.Property(x => x.Gender)
            .IsRequired();

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AutoRegistrationNumber)
        .HasMaxLength(100)
        .HasComputedColumnSql(
            "CONVERT(char(4), YEAR([CreatedAtUtc])) + '_' +" +
            " RIGHT('0' + CONVERT(varchar(2), MONTH([CreatedAtUtc])), 2) + '_' +" +
            " RIGHT('0' + CONVERT(varchar(2), DAY([CreatedAtUtc])), 2) + '_' +" +
            " CONVERT(varchar(20), [PersonId])",
            stored: true
        );

        builder.HasIndex(x => x.Phone)
            .IsUnique();

        builder.HasIndex(x => x.AutoRegistrationNumber)
            .IsUnique()
            .HasFilter(null);

        builder.HasIndex(x => x.FullName);
        builder.HasIndex(x => x.Birthdate);
        builder.HasIndex(x => x.NationalNo);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
