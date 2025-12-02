using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

public sealed record GetDisabledCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsExpired = null,
    int? PatientId = null,
    DateTime? ExpirationFrom = null,
    DateTime? ExpirationTo = null,
    string SortColumn = "ExpirationDate",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<DisabledCardDto>>>
{
    public string CacheKey =>
        $"disabledcards:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":expired={(IsExpired?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":expFrom={(ExpirationFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":expTo={(ExpirationTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["disabledcard"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
