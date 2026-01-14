using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public sealed class GetTicketsQueryHandler
    : IRequestHandler<GetTicketsQuery, Result<PaginatedList<TicketDto>>>
{
    private readonly IAppDbContext _context;

    public GetTicketsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TicketDto>>> Handle(
        GetTicketsQuery query,
        CancellationToken ct)
    {
        IQueryable<Ticket> ticketsQuery = _context.Tickets
            .Include(t => t.Patient!)
                .ThenInclude(p => p.Person)
            .Include(t => t.Service!)
                .ThenInclude(s => s.Department)
            .AsNoTracking();

        ticketsQuery = ApplyFilters(ticketsQuery, query);
        ticketsQuery = ApplySearch(ticketsQuery, query);
        ticketsQuery = ApplySorting(ticketsQuery, query);

        var totalCount = await ticketsQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var tickets = await ticketsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = tickets
            .Select(t => t.ToDto())
            .ToList();

        return new PaginatedList<TicketDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Ticket> ApplyFilters(
        IQueryable<Ticket> query,
        GetTicketsQuery q)
    {
        if (q.Status.HasValue)
        {
            query = query.Where(t => t.Status == q.Status.Value);
        }

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
        {
            var patientId = q.PatientId.Value;
            query = query.Where(t => t.PatientId == patientId);
        }

        if (q.ServiceId.HasValue && q.ServiceId.Value > 0)
        {
            var serviceId = q.ServiceId.Value;
            query = query.Where(t => t.ServiceId == serviceId);
        }

        if (q.DepartmentId.HasValue && q.DepartmentId.Value > 0)
        {
            var deptId = q.DepartmentId.Value;
            query = query.Where(t =>
                t.Service != null &&
                t.Service.DepartmentId == deptId);
        }

        if (q.CreatedFrom.HasValue)
        {
            var from = q.CreatedFrom.Value.Date;
            query = query.Where(t => t.CreatedAtUtc >= from);
        }

        if (q.CreatedTo.HasValue)
        {
            var to = q.CreatedTo.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(t => t.CreatedAtUtc <= to);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<Ticket> ApplySearch(
        IQueryable<Ticket> query,
        GetTicketsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm.Trim().ToLower()}%";

        return query.Where(t =>
            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(t.Patient.Person.FullName.ToLower(), pattern)) ||

            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(t.Patient.Person.Phone.ToLower(), pattern)) ||

            (t.Patient != null &&
             t.Patient.Person != null &&
             EF.Functions.Like(
                 t.Patient.Person.AutoRegistrationNumber!.ToLower(),
                 pattern)) ||

            (t.Service != null &&
             EF.Functions.Like(t.Service.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private static IQueryable<Ticket> ApplySorting(
        IQueryable<Ticket> query,
        GetTicketsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "createdat";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
