using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Services.Tickets;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public sealed class TicketsFilter : FilterSpecification<Ticket>
{
    private readonly GetTicketsQuery _q;

    public TicketsFilter(GetTicketsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Ticket> Apply(IQueryable<Ticket> query)
    {
        // Includes
        query = query
            .Include(t => t.Patient!)
                .ThenInclude(p => p.Person)
            .Include(t => t.Service)
                .ThenInclude(s => s.Department);

        query = ApplyFilters(query);
        
        query = ApplySearch(query);
        query = ApplySorting(query);

        System.Console.WriteLine(query.ToQueryString());

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Ticket> ApplyFilters(IQueryable<Ticket> query)
    {
        if (_q.Status.HasValue)
        {
            var status = _q.Status.Value;
            query = query.Where(t => t.Status == status);
        }

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
        {
            var patientId = _q.PatientId.Value;
            query = query.Where(t => t.PatientId == patientId);
        }

        if (_q.ServiceId.HasValue && _q.ServiceId.Value > 0)
        {
            var serviceId = _q.ServiceId.Value;
            query = query.Where(t => t.ServiceId == serviceId);
        }
        if (_q.DepartmentId.HasValue && _q.DepartmentId.Value > 0)
        {
            var deptId = _q.DepartmentId.Value;
            query = query.Where(t => t.Service != null &&
                                     t.Service.DepartmentId == deptId);
        }

        if (_q.CreatedFrom.HasValue)
        {
            var from = _q.CreatedFrom.Value.Date;
            query = query.Where(t => t.CreatedAtUtc >= from);
        }

        if (_q.CreatedTo.HasValue)
        {
            var to = _q.CreatedTo.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(t => t.CreatedAtUtc <= to);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<Ticket> ApplySearch(IQueryable<Ticket> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var term = _q.SearchTerm!.Trim().ToLower();
        var pattern = $"%{term}%";

        return query.Where(t =>
            // by patient name
            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(t.Patient.Person.FullName.ToLower(), pattern)) ||

            // by patient phone
            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(t.Patient.Person.Phone.ToLower(), pattern)) ||

            // by patient auto registration number
            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(t.Patient.Person.AutoRegistrationNumber!.ToLower(), pattern)) ||

            // by service name
            (t.Service != null &&
             EF.Functions.Like(t.Service.Name.ToLower(), pattern)));
    }

    // ---------------- SORTING ----------------
    private IQueryable<Ticket> ApplySorting(IQueryable<Ticket> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "createdat";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "patient" => isDesc
                ? query.OrderByDescending(t => t.Patient!.Person!.FullName)
                : query.OrderBy(t => t.Patient!.Person!.FullName),

            "service" => isDesc
                ? query.OrderByDescending(t => t.Service!.Name)
                : query.OrderBy(t => t.Service!.Name),

            "department" => isDesc
                ? query.OrderByDescending(t => t.Service!.Department!.Name)
                : query.OrderBy(t => t.Service!.Department!.Name),

            "status" => isDesc
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),

            "createdat" => isDesc
                ? query.OrderByDescending(t => t.CreatedAtUtc)
                : query.OrderBy(t => t.CreatedAtUtc),

            _ => isDesc
                ? query.OrderByDescending(t => t.CreatedAtUtc)
                : query.OrderBy(t => t.CreatedAtUtc),
        };
    }
}
