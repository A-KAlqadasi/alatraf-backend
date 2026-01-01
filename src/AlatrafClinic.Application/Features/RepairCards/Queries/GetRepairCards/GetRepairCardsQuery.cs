using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

using MediatR;

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
    string SortColumn = "repaircardid",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<RepairCardDiagnosisDto>>>;