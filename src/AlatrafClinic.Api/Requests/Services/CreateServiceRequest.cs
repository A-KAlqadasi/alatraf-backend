using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Services;

public sealed class CreateServiceRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;
    public int? DepartmentId { get; init; }
    public decimal? Price { get; init; }
}
