using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

public sealed record GetRepairCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsActive = null,
    bool? IsLate = null,
    RepairCardStatus? Status = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    string SortColumn = "DeliveryDate",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<RepairCardDto>>>
{
    public string CacheKey =>
        $"repaircards:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":active={(IsActive?.ToString() ?? "-")}" +
        $":late={(IsLate?.ToString() ?? "-")}" +
        $":status={(Status?.ToString() ?? "-")}" +
        $":diag={(DiagnosisId?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["repaircard"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
