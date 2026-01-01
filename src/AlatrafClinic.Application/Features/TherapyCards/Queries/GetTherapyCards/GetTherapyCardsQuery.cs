using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public sealed record GetTherapyCardsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    bool? IsActive = null,
    TherapyCardType? Type = null,
    TherapyCardStatus? Status = null,
    DateOnly? ProgramStartFrom = null,
    DateOnly? ProgramStartTo = null,
    DateOnly? ProgramEndFrom = null,
    DateOnly? ProgramEndTo = null,
    int? DiagnosisId = null,
    int? PatientId = null,
    string SortColumn = "ProgramStartDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<TherapyCardDto>>>;