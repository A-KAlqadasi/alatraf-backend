using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPaymentsWaitingList;

public sealed record GetPaymentsWaitingListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    PaymentReference? PaymentReference = null,
    bool? IsCompleted = null,
    string SortColumn = "CreatedAtUtc",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<PaymentWaitingListDto>>>
{
    public string CacheKey =>
        $"payments:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":ref={(PaymentReference?.ToString() ?? "-")}" +
        $":completed={(IsCompleted?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["payment"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
