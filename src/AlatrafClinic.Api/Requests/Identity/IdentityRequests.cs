using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Identity;

public sealed class CreateUserRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PersonId { get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string UserName { get; set; } = default!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = default!;

    public bool IsActive { get; set; }
}

public sealed class ActivateUserRequest
{
    [Required]
    public bool IsActive { get; set; }
}

public sealed class ResetPasswordRequest
{
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = default!;
}

public sealed class ChangeCredentialsRequest
{
    [Required]
    public string OldPassword { get; set; } = default!;

    [MinLength(8)]
    public string? NewPassword { get; set; }

    [MinLength(3)]
    [MaxLength(50)]
    public string? NewUsername { get; set; }
}

public sealed class CreateRoleRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Name { get; set; } = default!;
}

public sealed class PermissionIdsRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<int> PermissionIds { get; set; } = [];
}

