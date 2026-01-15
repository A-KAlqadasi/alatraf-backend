using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Identity;

public sealed class CreateUserRequest
{
     [Required(ErrorMessage = "Fullname is required.")]
    public string Fullname { get; set; } = default!;

    [Required(ErrorMessage ="Birthdate is required")]
    public DateOnly Birthdate { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(77|78|73|71|70)\d{7}$", ErrorMessage = "Phone number must start with 77, 78, 73, 71, or 70 and be 9 digits long.")]
    public string Phone { get; set; } = default!;

    [Required(ErrorMessage = "National number is required")]
    public string NationalNo {get; set; } = default!;
    [Required(ErrorMessage = "Address Id is required")]
    public int AddressId { get ;set; } = default!;
    [Required(ErrorMessage = "Gender is required")]
    public bool Gender {get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string UserName { get; set; } = default!;

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*()_\-+=\[\]{};:'"",.<>?/\\|`~]).{8,}$",
     ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter and one special character.")]
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

public sealed class AssignRolesRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<string> RoleIds { get; set; } = [];
}

public sealed class RemoveRolesRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<string> RoleIds { get; set; } = [];
}

public sealed class GetUserFilterRequest
{
    public string? searchBy { get; set; } 
    public bool? IsActive { get; set; }
}