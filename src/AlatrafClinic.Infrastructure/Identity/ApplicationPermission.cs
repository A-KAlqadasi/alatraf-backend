namespace AlatrafClinic.Infrastructure.Identity;

public class ApplicationPermission
{
    public int Id { get; set; }

    // e.g. "ticket:create"
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; } = new List<RolePermission>();
    public ICollection<UserPermissionOverride> UserPermissionOverrides { get; } = new List<UserPermissionOverride>();
}
