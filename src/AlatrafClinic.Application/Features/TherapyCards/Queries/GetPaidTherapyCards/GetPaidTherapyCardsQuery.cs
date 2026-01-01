using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetPaidTherapyCards;

public sealed record GetPaidTherapyCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string SortColumn = "PaymentDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<TherapyCardDiagnosisDto>>>;