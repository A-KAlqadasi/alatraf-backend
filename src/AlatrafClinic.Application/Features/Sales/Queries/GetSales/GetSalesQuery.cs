using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSales;

public sealed record GetSalesQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    SaleStatus? Status = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string SortColumn = "SaleDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<SaleDto>>>;