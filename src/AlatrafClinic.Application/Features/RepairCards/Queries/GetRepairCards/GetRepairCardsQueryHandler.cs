using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

public sealed class GetRepairCardsQueryHandler
    : IRequestHandler<GetRepairCardsQuery, Result<PaginatedList<RepairCardDiagnosisDto>>>
{
    private readonly IAppDbContext _context;

    public GetRepairCardsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<RepairCardDiagnosisDto>>> Handle(
        GetRepairCardsQuery query,
        CancellationToken ct)
    {
        IQueryable<RepairCard> cardsQuery = _context.RepairCards
            .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjurySides)
            .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjuryReasons)
            .Include(r=> r.Diagnosis).ThenInclude(d=> d.InjuryTypes)
            .Include(rc => rc.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(rc => rc.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.IndustrialPart)
            .Include(rc => rc.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.Unit)
            .AsNoTracking();

        cardsQuery = ApplyFilters(cardsQuery, query);
        cardsQuery = ApplySearch(cardsQuery, query);
        cardsQuery = ApplySorting(cardsQuery, query);

        var totalCount = await cardsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var cards = await cardsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = cards.ToDiagnosisDtos();

        return new PaginatedList<RepairCardDiagnosisDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<RepairCard> ApplyFilters(
        IQueryable<RepairCard> query,
        GetRepairCardsQuery q)
    {
        if (q.IsActive.HasValue)
            query = query.Where(rc => rc.IsActive == q.IsActive.Value);

        if (q.IsLate.HasValue)
            query = query.Where(rc => rc.IsLate == q.IsLate.Value);

        if (q.Status.HasValue)
            query = query.Where(rc => rc.Status == q.Status.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId.Value > 0)
            query = query.Where(rc => rc.DiagnosisId == q.DiagnosisId.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(rc =>
                rc.Diagnosis != null &&
                rc.Diagnosis.PatientId == q.PatientId.Value);

        return query;
    }

    private static IQueryable<RepairCard> ApplySearch(
        IQueryable<RepairCard> query,
        GetRepairCardsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(rc =>
            (rc.Diagnosis != null &&
                (EF.Functions.Like(rc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (rc.Diagnosis.Patient != null &&
                  rc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(rc.Diagnosis.Patient.Person.FullName.ToLower(), pattern))))
            ||
            rc.DiagnosisIndustrialParts.Any(p =>
                p.IndustrialPartUnit != null &&
                p.IndustrialPartUnit.IndustrialPart != null &&
                EF.Functions.Like(p.IndustrialPartUnit.IndustrialPart.Name.ToLower(), pattern))
        );
    }

    private static IQueryable<RepairCard> ApplySorting(
        IQueryable<RepairCard> query,
        GetRepairCardsQuery q)
    {
        var col    = q.SortColumn?.Trim().ToLowerInvariant() ?? "repaircardid";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "repaircardid" => isDesc
                ? query.OrderByDescending(rc => rc.Id)
                : query.OrderBy(rc => rc.Id),

            "status" => isDesc
                ? query.OrderByDescending(rc => rc.Status)
                : query.OrderBy(rc => rc.Status),

            "patient" => isDesc
                ? query.OrderByDescending(rc => rc.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(rc => rc.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(rc => rc.CreatedAtUtc)
        };
    }
}