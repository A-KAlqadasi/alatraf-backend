namespace AlatrafClinic.Infrastructure.Data.Configurations;

using AlatrafClinic.Domain.Sessions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("SessionId");

        builder.Property(s => s.IsTaken)
            .IsRequired();

        builder.Property(s => s.Number)
            .IsRequired();

        builder.Property(s => s.SessionDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(s => s.TherapyCardId)
            .IsRequired();

        builder.HasOne(s => s.TherapyCard)
            .WithMany(tc => tc.Sessions)
            .HasForeignKey(s => s.TherapyCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.SessionPrograms)
            .WithOne(sp => sp.Session)
            .HasForeignKey(sp => sp.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique session number per therapy card
        builder.HasIndex(s => new { s.TherapyCardId, s.Number })
            .IsUnique();

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
