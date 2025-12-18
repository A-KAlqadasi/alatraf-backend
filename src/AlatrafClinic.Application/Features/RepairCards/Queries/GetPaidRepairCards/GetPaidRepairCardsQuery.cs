using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPaidRepairCards;

public sealed record GetPaidRepairCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string SortColumn = "PaymentDate",
    string SortDirection = "asc"
) : IRequest<Result<PaginatedList<RepairCardDiagnosisDto>>>;