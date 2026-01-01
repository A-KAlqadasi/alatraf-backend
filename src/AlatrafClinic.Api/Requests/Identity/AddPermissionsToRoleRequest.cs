using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Identity;

public sealed class AddPermissionsToRoleRequest
{
    [Required]
    [StringLength(256, MinimumLength = 2)]
    public string RoleName { get; init; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one permission is required.")]
    public List<string> PermissionNames { get; init; } = new();
}

public sealed class RemovePermissionsFromRoleRequest
{
    [Required]
    [StringLength(256, MinimumLength = 2)]
    public string RoleName { get; init; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one permission is required.")]
    public List<string> PermissionNames { get; init; } = new();
}

public sealed class AddPermissionsToUserRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one permission is required.")]
    public List<string> PermissionNames { get; init; } = new();
}

public sealed class RemovePermissionsFromUserRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one permission is required.")]
    public List<string> PermissionNames { get; init; } = new();
}

public sealed class ChangeUserNameAndPasswordRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    [StringLength(12, MinimumLength = 4)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(12, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[A-Za-z]).{6,12}$",
    ErrorMessage = "Password must be 6–12 characters long and contain at least one letter.")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    [StringLength(12, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[A-Za-z]).{6,12}$",
    ErrorMessage = "Password must be 6–12 characters long and contain at least one letter.")]
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    public DateOnly Birthdate { get; init; }

    [Required]
    [Phone]
    [StringLength(30)]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string NationalNo { get; init; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 3)]
    public string Address { get; init; } = string.Empty;

    [Required]
    public bool Gender { get; init; }

    [Required]
    [StringLength(12, MinimumLength = 4)]
    public string UserName { get; init; } = string.Empty;

    [Required]
    [StringLength(12, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[A-Za-z]).{6,12}$",
    ErrorMessage = "Password must be 6–12 characters long and contain at least one letter.")]
    public string Password { get; init; } = string.Empty;

    public List<string> Permissions { get; init; } = new();
    public List<string> Roles { get; init; } = new();
}

public sealed class UpdateUserRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Fullname { get; init; } = string.Empty;

    [Required]
    public DateOnly Birthdate { get; init; }

    [Required]
    [Phone]
    [StringLength(30)]
    public string Phone { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string NationalNo { get; init; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 3)]
    public string Address { get; init; } = string.Empty;

    [Required]
    public bool Gender { get; init; }

    [Required]
    public bool IsActive { get; init; }
}

public sealed class UsersFilterRequest
{
    [StringLength(200)]
    public string? SearchTerm { get; init; }

    [StringLength(256)]
    public string? UserName { get; init; }

    [StringLength(200)]
    public string? FullName { get; init; }

    public bool? IsActive { get; init; }

    [StringLength(50)]
    public string SortColumn { get; init; } = "UserName";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be either 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "asc";
}