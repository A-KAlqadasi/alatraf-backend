using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<AppUser, IdentityRole, string>
{
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ApplicationPermission> Permissions => Set<ApplicationPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

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