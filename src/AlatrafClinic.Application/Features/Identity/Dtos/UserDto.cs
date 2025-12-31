
namespace AlatrafClinic.Application.Features.Identity.Dtos;


public class CreateUserRequest
{
    public int PersonId { get; set; }
    public bool IsActive { get; set; }
}

public class ResetPasswordRequest
{
    public string UserId { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}

public class UserListItemDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class UserDetailsDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public bool IsActive { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];
    public IReadOnlyList<string> PermissionOverrides { get; set; } = [];
}

public class CreateRoleRequest
{
    public string Name { get; set; } = default!;
}

public class AssignRoleRequest
{
    public string UserId { get; set; } = default!;
    public string RoleId { get; set; } = default!;
}

public class RolePermissionRequest
{
    public string RoleId { get; set; } = default!;
    public int PermissionId { get; set; }
}

public class RoleDetailsDto
{
    public string RoleId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public IReadOnlyList<string> Permissions { get; set; } = [];
}

public class UserPermissionOverrideRequest
{
    public string UserId { get; set; } = default!;
    public int PermissionId { get; set; }
}

public class UserPermissionOverrideDto
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = default!;
    public string Effect { get; set; } = default!;
}
