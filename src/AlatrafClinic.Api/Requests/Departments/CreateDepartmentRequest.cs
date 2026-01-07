using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Departments;

public sealed class UpdateDepartmentRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string NewName { get; init; } = string.Empty;
}