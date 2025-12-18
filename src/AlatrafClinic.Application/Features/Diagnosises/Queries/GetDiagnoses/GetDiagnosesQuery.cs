using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;

using MediatR;

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
    DateOnly? InjuryDateFrom = null,
    DateOnly? InjuryDateTo = null,
    DateOnly? CreatedDateFrom = null,
    DateOnly? CreatedDateTo = null
) : IRequest<Result<PaginatedList<DiagnosisDto>>>;