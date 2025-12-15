using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPaidRepairCards;

public sealed class GetPaidRepairCardsQueryHandler
    : IRequestHandler<GetPaidRepairCardsQuery, Result<PaginatedList<RepairCardDiagnosisDto>>>
{
    private readonly IAppDbContext _context;

    public GetPaidRepairCardsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<RepairCardDiagnosisDto>>> Handle(
        GetPaidRepairCardsQuery query,
        CancellationToken ct)
    {
        IQueryable<Diagnosis> diagnosesQuery = _context.Diagnoses
            .Include(d => d.Patient)
                .ThenInclude(p => p.Person)
            .Include(d => d.RepairCard)
            .Include(d => d.Payments)
            .Include(d => d.InjuryReasons)
            .Include(d => d.InjurySides)
            .Include(d => d.InjuryTypes)
            .Include(d => d.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.IndustrialPart)
            .Include(d => d.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.Unit)
            .Include(d => d.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Section)
            .AsNoTracking();

        diagnosesQuery = ApplyFilters(diagnosesQuery);
        diagnosesQuery = ApplySearch(diagnosesQuery, query);
        diagnosesQuery = ApplySorting(diagnosesQuery, query);

        var totalCount = await diagnosesQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var diagnoses = await diagnosesQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = diagnoses
            .Select(MapToDto)
            .ToList();

        return new PaginatedList<RepairCardDiagnosisDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Diagnosis> ApplyFilters(IQueryable<Diagnosis> query)
    {
        query = query.Where(d => d.DiagnoType == DiagnosisType.Limbs);

        query = query.Where(d => d.RepairCard != null);

        query = query.Where(d =>
            d.Payments.Any(p => p.IsCompleted && p.PaymentDate != null));

        return query;
    }

    private static IQueryable<Diagnosis> ApplySearch(
        IQueryable<Diagnosis> query,
        GetPaidRepairCardsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term = q.SearchTerm.Trim();

        if (int.TryParse(term, out var repairCardId))
        {
            return query.Where(d => d.RepairCard!.Id == repairCardId);
        }

        var pattern = $"%{term.ToLower()}%";

        return query.Where(d =>
            d.Patient != null &&
            d.Patient.Person != null &&
            EF.Functions.Like(d.Patient.Person.FullName.ToLower(), pattern));
    }

    private static IQueryable<Diagnosis> ApplySorting(
        IQueryable<Diagnosis> query,
        GetPaidRepairCardsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "paymentdate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "paymentdate" => isDesc
                ? query.OrderByDescending(d => d.Payments
                    .Where(p => p.IsCompleted && p.PaymentDate != null)
                    .Min(p => p.PaymentDate))
                : query.OrderBy(d => d.Payments
                    .Where(p => p.IsCompleted && p.PaymentDate != null)
                    .Min(p => p.PaymentDate)),

            "repaircardid" => isDesc
                ? query.OrderByDescending(d => d.RepairCard!.Id)
                : query.OrderBy(d => d.RepairCard!.Id),

            "patient" => isDesc
                ? query.OrderByDescending(d => d.Patient!.Person!.FullName)
                : query.OrderBy(d => d.Patient!.Person!.FullName),

            _ => query.OrderBy(d => d.Payments
                .Where(p => p.IsCompleted && p.PaymentDate != null)
                .Min(p => p.PaymentDate)),
        };
    }

    private static RepairCardDiagnosisDto MapToDto(Diagnosis d)
    {
        var person = d.Patient.Person;

        var industrialParts = d.DiagnosisIndustrialParts
            .Select(dip =>
            {
                var partName = dip.IndustrialPartUnit.IndustrialPart.Name;
                var unitName = dip.IndustrialPartUnit?.Unit?.Name;

                var sectionName = dip.DoctorSectionRoom != null
                    ? dip.DoctorSectionRoom.Section.Name
                    : null;

                return new DiagnosisIndustrialPartDto
                {
                    DiagnosisIndustrialPartId = dip.Id,
                    IndustrialPartId = dip.IndustrialPartUnit!.IndustrialPartId,
                    PartName = partName,
                    UnitId = dip.IndustrialPartUnit!.UnitId,
                    UnitName = unitName ?? string.Empty,
                    Quantity = dip.Quantity,
                    Price = dip.Price,
                    DoctorSectionRoomId = dip.DoctorSectionRoomId,
                    DoctorSectionName = sectionName,
                    DoctorAssignedDate = dip.DoctorAssignDate.HasValue
                        ? dip.DoctorAssignDate.Value
                        : null,
                    TotalPrice = dip.Price * dip.Quantity
                };
            })
            .ToList();

        var totalCost = industrialParts.Sum(x => x.TotalPrice);

        return new RepairCardDiagnosisDto
        {
            RepairCardId = d.RepairCard!.Id,
            TicketId = d.TicketId,
            PatientId = d.PatientId,
            PatientName = person.FullName,
            Gender = person.Gender ? "ذكر" : "أنثى",
            Age = UtilityService.CalculateAge(person.Birthdate, AlatrafClinicConstants.TodayDate),
            IsActive = d.RepairCard.IsActive,

            DiagnosisId = d.Id,
            DiagnosisText = d.DiagnosisText,
            InjuryDate = d.InjuryDate,
            DiagnosisType = d.DiagnoType.ToArabicDiagnosisType(),

            CardStatus = d.RepairCard.Status.ToArabicRepairCardStatus(),

            InjuryReasons = d.InjuryReasons
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            InjurySides = d.InjurySides
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            InjuryTypes = d.InjuryTypes
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            TotalCost = totalCost,
            DiagnosisIndustrialParts = industrialParts
        };
    }
}