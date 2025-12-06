using AlatrafClinic.Domain.TherapyCards.Sessions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlatrafClinic.Infrastructure.Data.Configurations;

public sealed class SessionProgramConfiguration : IEntityTypeConfiguration<SessionProgram>
{
    public void Configure(EntityTypeBuilder<SessionProgram> builder)
    {
        builder.ToTable("SessionPrograms");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
            .HasColumnName("SessionProgramId");

        builder.Property(sp => sp.DiagnosisProgramId)
            .IsRequired();

        builder.Property(sp => sp.SessionId)
            .IsRequired();

        builder.Property(sp => sp.DoctorSectionRoomId)
            .IsRequired();

        builder.HasOne(sp => sp.DiagnosisProgram)
            .WithMany(dp => dp.SessionPrograms) 
            .HasForeignKey(sp => sp.DiagnosisProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sp => sp.Session)
            .WithMany(s => s.SessionPrograms)
            .HasForeignKey(sp => sp.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sp => sp.DoctorSectionRoom)
            .WithMany(dsr => dsr.SessionPrograms)
            .HasForeignKey(sp => sp.DoctorSectionRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(sp => !sp.IsDeleted);
    }
}
