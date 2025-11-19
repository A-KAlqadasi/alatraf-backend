using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler
    : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PaymentDto>>> Handle(GetPaymentsQuery query, CancellationToken ct)
    {
        var paymentsQuery = await _unitOfWork.Payments.GetPaymentsQueryAsync();

        paymentsQuery = ApplyFilters(paymentsQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            paymentsQuery = ApplySearch(paymentsQuery, query.SearchTerm!);

        paymentsQuery = ApplySorting(paymentsQuery, query.SortColumn, query.SortDirection);

        // Paging guards
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await paymentsQuery.CountAsync(ct);

        var items = await paymentsQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => p.ToDto())
            .ToListAsync(ct);

        return new PaginatedList<PaymentDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Payment> ApplyFilters(IQueryable<Payment> query, GetPaymentsQuery q)
    {
        if (q.AccountKind.HasValue)
            query = query.Where(p => p.AccountKind == q.AccountKind.Value);

        if (q.IsCompleted.HasValue)
            query = query.Where(p => p.IsCompleted == q.IsCompleted.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId > 0)
            query = query.Where(p => p.DiagnosisId == q.DiagnosisId);

        if (q.PatientId.HasValue && q.PatientId > 0)
            query = query.Where(p => p.Diagnosis != null && p.Diagnosis.PatientId == q.PatientId);

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<Payment> ApplySearch(IQueryable<Payment> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(p =>
            (p.Diagnosis != null &&
             (EF.Functions.Like(p.Diagnosis.DiagnosisText.ToLower(), pattern) ||
              (p.Diagnosis.Patient != null && p.Diagnosis.Patient.Person != null &&
               EF.Functions.Like(p.Diagnosis.Patient.Person.FullName.ToLower(), pattern)))));
    }

    // ---------------- SORTING ----------------
    private static IQueryable<Payment> ApplySorting(IQueryable<Payment> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "createdatutc";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "totalamount" => isDesc ? query.OrderByDescending(p => p.TotalAmount) : query.OrderBy(p => p.TotalAmount),
            "paidamount" => isDesc ? query.OrderByDescending(p => p.PaidAmount) : query.OrderBy(p => p.PaidAmount),
            "patient" => isDesc
                ? query.OrderByDescending(p => p.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(p => p.Diagnosis!.Patient!.Person!.FullName),
            "completed" => isDesc ? query.OrderByDescending(p => p.IsCompleted) : query.OrderBy(p => p.IsCompleted),
            _ => isDesc ? query.OrderByDescending(p => p.CreatedAtUtc) : query.OrderBy(p => p.CreatedAtUtc)
        };
    }
}
