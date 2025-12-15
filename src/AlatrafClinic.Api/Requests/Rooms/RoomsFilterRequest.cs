using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Rooms;

public sealed class RoomsFilterRequest
{
    [StringLength(200)]
    public string? SearchTerm { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "SectionId must be greater than 0.")]
    public int? SectionId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than 0.")]
    public int? DepartmentId { get; init; }

    [StringLength(50)]
    public string SortColumn { get; init; } = "name";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be either 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "asc";
}
