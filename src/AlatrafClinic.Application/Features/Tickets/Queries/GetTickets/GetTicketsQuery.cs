using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public sealed record GetTicketsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    TicketStatus? Status = null,
    int? PatientId = null,
    int? ServiceId = null,
    int? DepartmentId = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    string SortColumn = "createdAt",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<TicketDto>>>;