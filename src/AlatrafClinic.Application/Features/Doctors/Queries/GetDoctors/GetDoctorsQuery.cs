
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctors;

public sealed record GetDoctorsQuery(
    int Page = 1,
    int PageSize = 20,
    int? DepartmentId = null,
    int? SectionId = null,
    int? RoomId = null,
    string? Search = null,
    string? Specialization = null,
    bool? HasActiveAssignment = null,
    string SortBy = "assigndate",
    string SortDir = "desc"
) : ICachedQuery<Result<PaginatedList<DoctorListItemDto>>>
{
    public string CacheKey =>
        $"doctors:p={Page}:ps={PageSize}" +
        $":dept={(DepartmentId?.ToString() ?? "-")}" +
        $":sec={(SectionId?.ToString() ?? "-")}" +
        $":room={(RoomId?.ToString() ?? "-")}" +
        $":search={(Search ?? "-")}" +
        $":spec={(Specialization ?? "-")}" +
        $":hasActive={(HasActiveAssignment?.ToString() ?? "-")}" +
        $":sort={SortBy}:{SortDir}";

    public string[] Tags => ["doctor"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}