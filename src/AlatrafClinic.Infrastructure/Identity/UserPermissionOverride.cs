namespace AlatrafClinic.Infrastructure.Identity;

public class UserPermissionOverride
{
    public string UserId { get; set; } = default!;
    public AppUser User { get; set; } = default!;
    public Effect Effect { get; set; }

    public int PermissionId { get; set; }
    public ApplicationPermission Permission { get; set; } = default!;
}

public enum Effect 
{
    Grant = 1,
    Deny
}