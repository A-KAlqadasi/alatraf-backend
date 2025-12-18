using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed record GetSessionsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? DoctorId = null,
    int? PatientId = null,
    int? TherapyCardId = null,
    bool? IsTaken = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string SortColumn = "SessionDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<SessionListDto>>>;