using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<AppUser, IdentityRole, string>
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ApplicationPermission> Permissions => Set<ApplicationPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<DiagnosisIndustrialPart> DiagnosisIndustrialParts => Set<DiagnosisIndustrialPart>();
    public DbSet<DiagnosisProgram> DiagnosisPrograms => Set<DiagnosisProgram>();

    public DbSet<IndustrialPart> IndustrialParts => Set<IndustrialPart>();
    public DbSet<IndustrialPartUnit> IndustrialPartUnits => Set<IndustrialPartUnit>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<RepairCard> RepairCards => Set<RepairCard>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PatientPayment> PatientPayments => Set<PatientPayment>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationPermission>(b =>
        {
            b.ToTable("Permissions");
            b.HasKey(p => p.Id);
            b.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            b.HasIndex(p => p.Name).IsUnique();
        });

        builder.Entity<RolePermission>(b =>
        {
            b.ToTable("RolePermissions");
            b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            b.HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId);

            b.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        });

        builder.Entity<UserPermission>(b =>
        {
            b.ToTable("UserPermissions");
            b.HasKey(up => new { up.UserId, up.PermissionId });

            b.HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId);

            b.HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId);
        });

        builder.Entity<RefreshToken>(b =>
        {
            b.ToTable("RefreshTokens");
            b.HasKey(rt => rt.Id);

            b.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(512);

            b.Property(rt => rt.UserId)
                .IsRequired()
                .HasMaxLength(450);
        });
    }
}