using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed class GetAppointmentsQueryHandler
    : IRequestHandler<GetAppointmentsQuery, Result<PaginatedList<AppointmentDto>>>
{
    private readonly IAppDbContext _context;

    public GetAppointmentsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<AppointmentDto>>> Handle(
        GetAppointmentsQuery query,
        CancellationToken ct)
    {
        // Base query + includes
        IQueryable<Appointment> appointmentsQuery = _context.Appointments
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Patient!)
                    .ThenInclude(p => p.Person)
            .AsNoTracking();

        // Apply filters/search/sorting
        appointmentsQuery = ApplyFilters(appointmentsQuery, query);
        appointmentsQuery = ApplySearch(appointmentsQuery, query);
        appointmentsQuery = ApplySorting(appointmentsQuery, query);

        // Total count AFTER filters/search, BEFORE paging
        var totalCount = await appointmentsQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var skip = (page - 1) * pageSize;

        // Paging
        var entities = await appointmentsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // Mapping – same as your existing mapping
        var items = entities.ToDtos();

        return new PaginatedList<AppointmentDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // ------------- FILTERS -------------
    private static IQueryable<Appointment> ApplyFilters(
        IQueryable<Appointment> query,
        GetAppointmentsQuery q)
    {
        if (q.Status.HasValue)
            query = query.Where(a => a.Status == q.Status.Value);

        if (q.PatientType.HasValue)
            query = query.Where(a => a.PatientType == q.PatientType.Value);

        if (q.FromDate.HasValue)
        {
            var from = q.FromDate.Value;
            query = query.Where(a => a.AttendDate >= from);
        }

        if (q.ToDate.HasValue)
        {
            // keep same semantics as your filter:
            // ToDate inclusive (end of day)
            var to = q.ToDate.Value;
            query = query.Where(a => a.AttendDate <= to);
        }

        return query;
    }

    // ------------- SEARCH -------------
    private static IQueryable<Appointment> ApplySearch(
        IQueryable<Appointment> query,
        GetAppointmentsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term    = q.SearchTerm.Trim();
        var pattern = $"%{term.ToLower()}%";

        return query.Where(a =>
            (a.Notes != null &&
             EF.Functions.Like(a.Notes.ToLower(), pattern))
            ||
            (a.Ticket != null &&
             a.Ticket.Patient != null &&
             a.Ticket.Patient.Person != null &&
             EF.Functions.Like(a.Ticket.Patient.Person.FullName.ToLower(), pattern)));
    }

    // ------------- SORTING -------------
    private static IQueryable<Appointment> ApplySorting(
        IQueryable<Appointment> query,
        GetAppointmentsQuery q)
    {
        var col    = q.SortColumn?.Trim().ToLowerInvariant() ?? "attenddate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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