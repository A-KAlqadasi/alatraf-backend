using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;


namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler
    : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentListItemDto>>>
{
    private readonly IAppDbContext _context;

    public GetPaymentsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<PaymentListItemDto>>> Handle(GetPaymentsQuery query, CancellationToken ct)
    {
        IQueryable<Payment> payments = _context.Payments
            .Include(p => p.PatientPayment)
            .Include(p => p.DisabledPayment)
            .Include(p => p.WoundedPayment)
            .AsNoTracking();

        payments = ApplyFilters(payments, query);
        payments = ApplySearch(payments, query);
        payments = ApplySorting(payments, query);

        var totalCount = await payments.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await payments
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new PaymentListItemDto
            {
                PaymentId = p.Id,
                TicketId = p.TicketId,
                DiagnosisId = p.DiagnosisId,
                PaymentReference = p.PaymentReference,
                AccountKind = p.AccountKind,
                IsCompleted = p.IsCompleted,
                PaymentDate = p.PaymentDate,
                TotalAmount = p.TotalAmount,
                PaidAmount = p.PaidAmount,
                Discount = p.Discount,
                Residual = Math.Max(0m, p.TotalAmount - ((p.PaidAmount ?? 0m) + (p.Discount ?? 0m))),

                VoucherNumber = p.PatientPayment != null ? p.PatientPayment.VoucherNumber : null,
                DisabledCardId = p.DisabledPayment != null ? p.DisabledPayment.DisabledCardId : null,
                WoundedCardId = p.WoundedPayment != null ? p.WoundedPayment.WoundedCardId : null
            })
            .ToListAsync(ct);

        return new PaginatedList<PaymentListItemDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Payment> ApplyFilters(IQueryable<Payment> q, GetPaymentsQuery f)
    {
        if (f.TicketId.HasValue && f.TicketId.Value > 0)
            q = q.Where(p => p.TicketId == f.TicketId.Value);

        if (f.DiagnosisId.HasValue && f.DiagnosisId.Value > 0)
            q = q.Where(p => p.DiagnosisId == f.DiagnosisId.Value);

        if (f.PaymentReference.HasValue)
            q = q.Where(p => p.PaymentReference == f.PaymentReference.Value);

        if (f.AccountKind.HasValue)
            q = q.Where(p => p.AccountKind == f.AccountKind.Value);

        if (f.IsCompleted.HasValue)
            q = q.Where(p => p.IsCompleted == f.IsCompleted.Value);

        if (f.PaymentDateFrom.HasValue)
            q = q.Where(p => p.PaymentDate != null && p.PaymentDate.Value >= f.PaymentDateFrom.Value);

        if (f.PaymentDateTo.HasValue)
            q = q.Where(p => p.PaymentDate != null && p.PaymentDate.Value <= f.PaymentDateTo.Value);

        return q;
    }

    private static IQueryable<Payment> ApplySearch(IQueryable<Payment> q, GetPaymentsQuery f)
    {
        if (string.IsNullOrWhiteSpace(f.SearchTerm))
            return q;

        var term = f.SearchTerm.Trim();

        // Numeric -> try match PaymentId or TicketId
        if (int.TryParse(term, out var n))
        {
            return q.Where(p => p.Id == n || p.TicketId == n || p.DiagnosisId == n);
        }

        // Text -> search voucher/report (most common)
        var pattern = $"%{term.ToLowerInvariant()}%";

        return q.Where(p =>
            (p.PatientPayment != null && EF.Functions.Like(p.PatientPayment.VoucherNumber.ToLower(), pattern)) ||
            (p.WoundedPayment != null && p.WoundedPayment.ReportNumber != null &&
             EF.Functions.Like(p.WoundedPayment.ReportNumber.ToLower(), pattern)));
    }

    private static IQueryable<Payment> ApplySorting(IQueryable<Payment> q, GetPaymentsQuery f)
    {
        var col = f.SortColumn?.Trim().ToLowerInvariant() ?? "paymentdate";
        var isDesc = string.Equals(f.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "paymentid" or "id" => isDesc ? q.OrderByDescending(p => p.Id) : q.OrderBy(p => p.Id),
            "ticketid" => isDesc ? q.OrderByDescending(p => p.TicketId) : q.OrderBy(p => p.TicketId),
            "totalamount" => isDesc ? q.OrderByDescending(p => p.TotalAmount) : q.OrderBy(p => p.TotalAmount),
            "iscompleted" => isDesc ? q.OrderByDescending(p => p.IsCompleted) : q.OrderBy(p => p.IsCompleted),
            "paymentdate" => isDesc ? q.OrderByDescending(p => p.PaymentDate) : q.OrderBy(p => p.PaymentDate),
            _ => isDesc ? q.OrderByDescending(p => p.PaymentDate) : q.OrderBy(p => p.PaymentDate)
        };
    }
}