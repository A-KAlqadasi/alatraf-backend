namespace AlatrafClinic.Infrastructure.Identity;

public class UserPermission
{
    public string UserId { get; set; } = default!;
    public AppUser User { get; set; } = default!;

    public int PermissionId { get; set; }
    public ApplicationPermission Permission { get; set; } = default!;
}