using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;


namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public sealed record GetTherapyCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsActive = null,
    TherapyCardType? Type = null,
    TherapyCardStatus? Status = null,
    DateOnly? ProgramStartFrom = null,
    DateOnly? ProgramStartTo = null,
    DateOnly? ProgramEndFrom = null,
    DateOnly? ProgramEndTo = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    string SortColumn = "ProgramStartDate",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<TherapyCardDto>>>
{
    public string CacheKey =>
        $"therapycards:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":type={(Type?.ToString() ?? "-")}" +
        $":active={(IsActive?.ToString() ?? "-")}" +
        $":status={(Status?.ToString() ?? "-")}" +
        $":startFrom={(ProgramStartFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":startTo={(ProgramStartTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":endFrom={(ProgramEndFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":endTo={(ProgramEndTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":diag={(DiagnosisId?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["therapy-card"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}
