using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed record GetPaymentsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    AccountKind? AccountKind = null,
    bool? IsCompleted = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    string SortColumn = "CreatedAtUtc",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<PaymentDto>>>
{
    public string CacheKey =>
        $"payments:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":kind={(AccountKind?.ToString() ?? "-")}" +
        $":completed={(IsCompleted?.ToString() ?? "-")}" +
        $":diag={(DiagnosisId?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["payment"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}