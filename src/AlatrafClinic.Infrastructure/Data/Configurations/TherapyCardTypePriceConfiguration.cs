using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class TherapyCardTypePriceConfiguration : IEntityTypeConfiguration<TherapyCardTypePrice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TherapyCardTypePrice> builder)
    {
        builder.ToTable("TherapyCardTypePrices");

        builder.HasKey(tctp => tctp.Id);

        builder.Property(tctp => tctp.Id)
            .HasColumnName("Id");

        builder.Property(tctp => tctp.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("Type")
            .HasMaxLength(50);

        builder.Property(tctp => tctp.SessionPrice)
            .IsRequired()
            .HasColumnName("SessionPrice")
            .HasColumnType("decimal(18,2)");
        
    }
}