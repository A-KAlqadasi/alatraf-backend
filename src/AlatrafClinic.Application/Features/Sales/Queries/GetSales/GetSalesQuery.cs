using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales.Enums;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSales;

public sealed record GetSalesQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    SaleStatus? Status = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string SortColumn = "SaleDate",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<SaleDto>>>
{
    public string CacheKey =>
        $"sales:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":status={(Status?.ToString() ?? "-")}" +
        $":diag={(DiagnosisId?.ToString() ?? "-")}" +
        $":pat={(PatientId?.ToString() ?? "-")}" +
        $":from={(FromDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":to={(ToDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["sale"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
