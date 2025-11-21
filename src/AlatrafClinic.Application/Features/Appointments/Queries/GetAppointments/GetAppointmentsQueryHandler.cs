using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;

using MediatR;

using Microsoft.EntityFrameworkCore;

public sealed class GetAppointmentsQueryHandler
    : IRequestHandler<GetAppointmentsQuery, Result<PaginatedList<AppointmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<AppointmentDto>>> Handle(GetAppointmentsQuery query, CancellationToken ct)
    {
        var appointmentsQuery = await _unitOfWork.Appointments.GetAppointmentsQueryAsync();

        appointmentsQuery = ApplyFilters(appointmentsQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            appointmentsQuery = ApplySearch(appointmentsQuery, query.SearchTerm!);

        appointmentsQuery = ApplySorting(appointmentsQuery, query.SortColumn, query.SortDirection);

        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await appointmentsQuery.CountAsync(ct);

        var items = await appointmentsQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                TicketId = a.TicketId,
                Ticket = a.Ticket != null ? a.Ticket.ToDto() : null,
                PatientType = a.PatientType,
                AttendDate = a.AttendDate,
                CreatedAt = a.CreatedAtUtc.DateTime,
                Status = a.Status,
                Notes = a.Notes,
                IsEditable = a.IsEditable,
                IsAppointmentTomorrow = a.IsAppointmentTomorrow()
            })
            .ToListAsync(ct);

        return new PaginatedList<AppointmentDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    private static IQueryable<Appointment> ApplyFilters(IQueryable<Appointment> query, GetAppointmentsQuery q)
    {
        if (q.Status.HasValue)
            query = query.Where(a => a.Status == q.Status.Value);

        if (q.PatientType.HasValue)
            query = query.Where(a => a.PatientType == q.PatientType.Value);

        if (q.FromDate.HasValue)
            query = query.Where(a => a.AttendDate.Date >= q.FromDate.Value.Date);

        if (q.ToDate.HasValue)
            query = query.Where(a => a.AttendDate.Date <= q.ToDate.Value.Date);

        return query;
    }

    private static IQueryable<Appointment> ApplySearch(IQueryable<Appointment> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(a =>
            a.Notes != null && EF.Functions.Like(a.Notes.ToLower(), pattern) ||
            a.Ticket != null && a.Ticket.Patient != null &&
            a.Ticket.Patient.Person != null &&
            EF.Functions.Like(a.Ticket.Patient.Person.FullName.ToLower(), pattern)
        );
    }

    private static IQueryable<Appointment> ApplySorting(IQueryable<Appointment> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "attenddate";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "attenddate" => isDesc
                ? query.OrderByDescending(a => a.AttendDate)
                : query.OrderBy(a => a.AttendDate),
            "status" => isDesc
                ? query.OrderByDescending(a => a.Status)
                : query.OrderBy(a => a.Status),
            "patient" => isDesc
                ? query.OrderByDescending(a => a.Ticket!.Patient!.Person!.FullName)
                : query.OrderBy(a => a.Ticket!.Patient!.Person!.FullName),
            _ => query.OrderByDescending(a => a.CreatedAtUtc)
        };
    }
}
