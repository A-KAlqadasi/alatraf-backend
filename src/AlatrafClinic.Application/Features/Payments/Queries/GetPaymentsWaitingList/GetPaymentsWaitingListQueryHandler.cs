using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPaymentsWaitingList;

public class GetPaymentsWaitingListQueryHandler
    : IRequestHandler<GetPaymentsWaitingListQuery, Result<PaginatedList<PaymentWaitingListDto>>>
{
    private readonly IAppDbContext _context;

    public GetPaymentsWaitingListQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<PaymentWaitingListDto>>> Handle(
        GetPaymentsWaitingListQuery query,
        CancellationToken ct)
    {
        IQueryable<Payment> paymentsQuery = _context.Payments
            .Include(p => p.Diagnosis)
                .ThenInclude(p=> p.TherapyCard)
            .Include(p => p.Diagnosis)
                .ThenInclude(p=> p.RepairCard)
            .Include(p => p.Diagnosis)
                .ThenInclude(p=> p.Sale)
            .Include(p=> p.Diagnosis)
            .ThenInclude(d => d.Patient)
                    .ThenInclude(pat => pat.Person)
            .AsNoTracking();

        paymentsQuery = ApplyFilters(paymentsQuery, query);
        paymentsQuery = ApplySearch(paymentsQuery, query);
        paymentsQuery = ApplySorting(paymentsQuery, query);

        var totalCount = await paymentsQuery.CountAsync(ct);

        var page = query.Page;
        var pageSize = query.PageSize;
        var skip = (page - 1) * pageSize;

        var payments = await paymentsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = payments.ToPaymentWaitingListDtos();

        return new PaginatedList<PaymentWaitingListDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Payment> ApplyFilters(
        IQueryable<Payment> query,
        GetPaymentsWaitingListQuery q)
    {
        if (q.IsCompleted.HasValue)
            query = query.Where(p => p.IsCompleted == q.IsCompleted.Value);

        if (q.PaymentReference.HasValue)
            query = query.Where(p => p.PaymentReference == q.PaymentReference.Value);

        return query;
    }

    private static IQueryable<Payment> ApplySearch(
        IQueryable<Payment> query,
        GetPaymentsWaitingListQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(p =>
            p.Diagnosis != null &&
            (
                (p.Diagnosis.Patient != null &&
                 p.Diagnosis.Patient.Person != null &&
                 EF.Functions.Like(p.Diagnosis.Patient.Person.FullName.ToLower(), pattern))
                 ||
                 (p.Diagnosis.Patient != null &&
                 p.Diagnosis.Patient.Person != null &&
                 EF.Functions.Like(p.Diagnosis.Patient.Person.Phone.ToLower(), pattern))
            ));
    }

    private static IQueryable<Payment> ApplySorting(
        IQueryable<Payment> query,
        GetPaymentsWaitingListQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "createdatutc";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {

            "patient" => isDesc
                ? query.OrderByDescending(p => p.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(p => p.Diagnosis!.Patient!.Person!.FullName),

            "completed" => isDesc
                ? query.OrderByDescending(p => p.IsCompleted)
                : query.OrderBy(p => p.IsCompleted),

            _ => isDesc
                ? query.OrderByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.CreatedAtUtc),
        };
    }
}
