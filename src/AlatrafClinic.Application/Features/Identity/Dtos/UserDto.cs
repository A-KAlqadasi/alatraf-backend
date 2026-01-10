
namespace AlatrafClinic.Application.Features.Identity.Dtos;


public class UserListItemDto
{
    public string UserId { get; set; } = default!;
    public string PersonName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Username { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class UserDetailsDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public bool IsActive { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
    public IReadOnlyList<string> Permissions { get; set; } = [];
}

public class RoleDetailsDto
{
    public string RoleId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public IReadOnlyList<string> Permissions { get; set; } = [];
}

public class PermissionDto
{
    public int PermissionId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class UserCreatedDto
{
    public string UserId { get; set; } = default!;
}