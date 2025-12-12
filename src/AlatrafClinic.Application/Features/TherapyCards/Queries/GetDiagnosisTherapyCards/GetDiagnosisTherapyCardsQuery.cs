using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public sealed record GetTherapyDiagnosesQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string SortColumn = "PaymentDate",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<TherapyCardDiagnosisDto>>>
{
    public string CacheKey =>
        $"therapydiagnoses:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["therapy-diagnosis"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
