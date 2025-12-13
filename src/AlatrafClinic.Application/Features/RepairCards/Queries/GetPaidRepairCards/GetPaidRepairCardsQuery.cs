using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPaidRepairCards;

public sealed record GetPaidRepairCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string SortColumn = "PaymentDate",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<RepairCardDiagnosisDto>>>
{
    public string CacheKey =>
        $"repairdiagnoses:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["repair-diagnosis"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
