using Microsoft.AspNetCore.Identity;

namespace AlatrafClinic.Infrastructure.Identity;

public class RolePermission
{
    public string RoleId { get; set; } = default!;
    public IdentityRole Role { get; set; } = default!;

    public int PermissionId { get; set; }
    public ApplicationPermission Permission { get; set; } = default!;
    public bool IsActive { get; set; }
}
