using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

public sealed record GetDisabledCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsExpired = null,
    int? PatientId = null,
    DateOnly? IssueDateFrom = null,
    DateOnly? IssueDateTo = null,
    DateOnly? ExpirationFrom = null,
    DateOnly? ExpirationTo = null,
    string SortColumn = "ExpirationDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<DisabledCardDto>>>;