using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Sections;

public sealed class SectionFilterRequest
{
    [MaxLength(200)]
    public string? SearchTerm { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than 0.")]
    public int? DepartmentId { get; init; }

    [Required]
    [MaxLength(50)]
    public string SortColumn { get; init; } = "name";

    [Required]
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "asc";
}
