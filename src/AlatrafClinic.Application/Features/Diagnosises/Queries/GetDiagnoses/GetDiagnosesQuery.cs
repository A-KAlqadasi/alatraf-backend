using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public sealed record GetDiagnosesQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string SortColumn = "createdAt",
    string SortDirection = "desc",
    DiagnosisType? Type = null,
    int? PatientId = null,
    int? TicketId = null,
    bool? HasRepairCard = null,
    bool? HasTherapyCards = null,
    bool? HasSale = null,
    DateTime? InjuryDateFrom = null,
    DateTime? InjuryDateTo = null,
    DateTime? CreatedDateFrom = null,
    DateTime? CreatedDateTo = null
) : ICachedQuery<Result<PaginatedList<DiagnosisDto>>>
{
    public string CacheKey =>
        $"diagnoses:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}" +
        $":type={(Type?.ToString() ?? "-")}" +
        $":patient={(PatientId?.ToString() ?? "-")}" +
        $":ticket={(TicketId?.ToString() ?? "-")}" +
        $":repair={(HasRepairCard?.ToString() ?? "-")}" +
        $":therapy={(HasTherapyCards?.ToString() ?? "-")}" +
        $":sale={(HasSale?.ToString() ?? "-")}" +
        $":injFrom={(InjuryDateFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":injTo={(InjuryDateTo?.ToString("yyyyMMdd") ?? "-")}" +
        $":crFrom={(CreatedDateFrom?.ToString("yyyyMMdd") ?? "-")}" +
        $":crTo={(CreatedDateTo?.ToString("yyyyMMdd") ?? "-")}";

    public string[] Tags => ["diagnosis"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}