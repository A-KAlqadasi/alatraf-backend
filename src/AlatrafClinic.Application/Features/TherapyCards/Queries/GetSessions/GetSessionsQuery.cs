using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed record GetSessionsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? DoctorId = null,
    int? PatientId = null,
    int? TherapyCardId = null,
    bool? IsTaken = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string SortColumn = "SessionDate",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<SessionListDto>>>
{
    public string CacheKey =>
        $"sessions:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":doctor={(DoctorId?.ToString() ?? "-")}" +
        $":patient={(PatientId?.ToString() ?? "-")}" +
        $":therapy={(TherapyCardId?.ToString() ?? "-")}" +
        $":taken={(IsTaken?.ToString() ?? "-")}" +
        $":from={(FromDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":to={(ToDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["session"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
