using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Services.Appointments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed class AppointmentsFilter : FilterSpecification<Appointment>
{
    private readonly GetAppointmentsQuery _q;

    public AppointmentsFilter(GetAppointmentsQuery q) : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Appointment> Apply(IQueryable<Appointment> query)
    {
        query = query
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Patient)
                    .ThenInclude(p => p.Person);

        // Filters
        if (_q.Status.HasValue)
            query = query.Where(a => a.Status == _q.Status.Value);

        if (_q.PatientType.HasValue)
            query = query.Where(a => a.PatientType == _q.PatientType.Value);

        if (_q.FromDate.HasValue)
        {
            var from = _q.FromDate.Value.Date;
            query = query.Where(a => a.AttendDate >= from);
        }

        if (_q.ToDate.HasValue)
        {
            var to = _q.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(a => a.AttendDate <= to);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(_q.SearchTerm))
        {
            var term = _q.SearchTerm.Trim();
            var pattern = $"%{term.ToLower()}%";

            query = query.Where(a =>
                a.Notes != null && EF.Functions.Like(a.Notes.ToLower(), pattern)
                ||
                a.Ticket != null && a.Ticket.Patient != null &&
                a.Ticket.Patient.Person != null &&
                EF.Functions.Like(a.Ticket.Patient.Person.FullName.ToLower(), pattern));
        }

        // Sorting
        query = ApplySorting(query, _q.SortColumn, _q.SortDirection);

        return query;
    }

    private static IQueryable<Appointment> ApplySorting(
        IQueryable<Appointment> query,
        string sortColumn,
        string sortDirection)
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