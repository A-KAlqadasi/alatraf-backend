using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.MedicalPrograms;

public sealed class MedicalProgramsFilterRequest
{
    public string? SearchTerm { get; init; }

    [Range(1, int.MaxValue)]
    public int? SectionId { get; init; }

    public bool? HasSection { get; init; }

    public string SortColumn { get; init; } = "Name";

    [RegularExpression("^(asc|desc)$")]
    public string SortDirection { get; init; } = "asc";
}
