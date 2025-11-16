using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

public sealed class GetRepairCardsQueryHandler
    : IRequestHandler<GetRepairCardsQuery, Result<PaginatedList<RepairCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRepairCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<RepairCardDto>>> Handle(GetRepairCardsQuery query, CancellationToken ct)
    {
        var repairQuery = await _unitOfWork.RepairCards.GetRepairCardsQueryAsync();

        repairQuery = ApplyFilters(repairQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            repairQuery = ApplySearch(repairQuery, query.SearchTerm!);

        repairQuery = ApplySorting(repairQuery, query.SortColumn, query.SortDirection);

        // Paging guards
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await repairQuery.CountAsync(ct);

        var items = await repairQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(rc => new RepairCardDto
            {
                RepairCardId = rc.Id,
                Diagnosis = rc.Diagnosis != null ? rc.Diagnosis.ToDto() : new DiagnosisDto(),
                IsActive = rc.IsActive,
                IsLate = rc.IsLate,
                CardStatus = rc.Status,
                DeliveryDate = rc.DeliveryTime != null ? rc.DeliveryTime.DeliveryDate : default,
                DiagnosisIndustrialParts = rc.DiagnosisIndustrialParts.Select(dip => new DiagnosisIndustrialPartDto
                {
                    DiagnosisIndustrialPartId = dip.Id,
                    IndustrialPartId = dip.IndustrialPartUnit!.IndustrialPartId,
                    PartName = dip.IndustrialPartUnit.IndustrialPart!.Name,
                    UnitId = dip.IndustrialPartUnit.UnitId,
                    UnitName = dip.IndustrialPartUnit.Unit!.Name ?? string.Empty,
                    Quantity = dip.Quantity,
                    Price = dip.Price
                }).ToList(),
                TotalCost = rc.DiagnosisIndustrialParts.Sum(dip => dip.Price * dip.Quantity)
            })
            .ToListAsync(ct);

        return new PaginatedList<RepairCardDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<RepairCard> ApplyFilters(IQueryable<RepairCard> query, GetRepairCardsQuery q)
    {
        if (q.IsActive.HasValue)
            query = query.Where(rc => rc.IsActive == q.IsActive.Value);

        if (q.IsLate.HasValue)
            query = query.Where(rc => rc.IsLate == q.IsLate.Value);

        if (q.Status.HasValue)
            query = query.Where(rc => rc.Status == q.Status.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId > 0)
            query = query.Where(rc => rc.DiagnosisId == q.DiagnosisId);

        if (q.PatientId.HasValue && q.PatientId > 0)
            query = query.Where(rc => rc.Diagnosis != null && rc.Diagnosis.PatientId == q.PatientId);

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<RepairCard> ApplySearch(IQueryable<RepairCard> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(rc =>
            (rc.Diagnosis != null &&
                (EF.Functions.Like(rc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (rc.Diagnosis.Patient != null && rc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(rc.Diagnosis.Patient.Person.FullName.ToLower(), pattern)))) ||
            rc.DiagnosisIndustrialParts.Any(p =>
                EF.Functions.Like(p.IndustrialPartUnit!.IndustrialPart!.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private static IQueryable<RepairCard> ApplySorting(IQueryable<RepairCard> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "deliverydate";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "deliverydate" => isDesc
                ? query.OrderByDescending(rc => rc.DeliveryTime!.DeliveryDate)
                : query.OrderBy(rc => rc.DeliveryTime!.DeliveryDate),

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
