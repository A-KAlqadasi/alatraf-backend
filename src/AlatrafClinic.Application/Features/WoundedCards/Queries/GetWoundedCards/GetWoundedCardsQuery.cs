using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;


namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

public sealed record GetWoundedCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsExpired = null,
    int? PatientId = null,
    DateOnly? IssueDateFrom = null,
    DateOnly? IssueDateTo = null,
    DateOnly? ExpirationFrom = null,
    DateOnly? ExpirationTo = null,
    string SortColumn = "Expiration",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<WoundedCardDto>>>;