using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

public sealed record GetWoundedCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsExpired = null,
    int? PatientId = null,
    DateOnly? ExpirationFrom = null,
    DateOnly? ExpirationTo = null,
    string SortColumn = "Expiration",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<WoundedCardDto>>>
{
    public string CacheKey =>
        $"woundedcards:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":expired={(IsExpired?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":expFrom={(ExpirationFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":expTo={(ExpirationTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["woundedcard"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}