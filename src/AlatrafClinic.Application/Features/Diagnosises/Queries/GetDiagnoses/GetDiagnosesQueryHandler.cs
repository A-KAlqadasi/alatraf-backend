using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public class GetDiagnosesQueryHandler
    : IRequestHandler<GetDiagnosesQuery, Result<PaginatedList<DiagnosisDto>>>
{
    private readonly IAppDbContext _context;

    public GetDiagnosesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<DiagnosisDto>>> Handle(
        GetDiagnosesQuery query,
        CancellationToken ct)
    {
        // Base query + includes
        IQueryable<Diagnosis> diagnosesQuery = _context.Diagnoses
            .Include(d => d.Patient)
                .ThenInclude(p => p.Person)
            .Include(d => d.Ticket)
            .Include(d => d.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .Include(d => d.DiagnosisIndustrialParts)
                .ThenInclude(di => di.IndustrialPartUnit)
                    .ThenInclude(u => u.IndustrialPart)
            .Include(d => d.RepairCard)
            .Include(d => d.Sale)
            .Include(d => d.TherapyCard)
            .AsNoTracking();

        // Apply filters/search/sorting
        diagnosesQuery = ApplyFilters(diagnosesQuery, query);
        diagnosesQuery = ApplySearch(diagnosesQuery, query);
        diagnosesQuery = ApplySorting(diagnosesQuery, query);

        var totalCount = await diagnosesQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var diagnoses = await diagnosesQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = diagnoses
            .Select(d => new DiagnosisDto
            {
                DiagnosisId   = d.Id,
                DiagnosisText = d.DiagnosisText,
                InjuryDate    = d.InjuryDate,

                TicketId      = d.TicketId,
                PatientId     = d.PatientId,
                PatientName   = d.Patient != null && d.Patient.Person != null
                                    ? d.Patient.Person.FullName
                                    : string.Empty,

                DiagnosisType = d.DiagnoType.ToArabicDiagnosisType(),

                InjuryReasons = d.InjuryReasons.ToDtos(),
                InjurySides   = d.InjurySides.ToDtos(),
                InjuryTypes   = d.InjuryTypes.ToDtos(),

                HasRepairCard   = d.RepairCard != null,
                HasSale         = d.Sale != null,
                HasTherapyCards = d.TherapyCard != null,
            })
            .ToList();

        var result = new PaginatedList<DiagnosisDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return result;
    }

    // -------------------- FILTERS --------------------
    private static IQueryable<Diagnosis> ApplyFilters(
        IQueryable<Diagnosis> query,
        GetDiagnosesQuery q)
    {
        if (q.Type.HasValue)
            query = query.Where(d => d.DiagnoType == q.Type.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(d => d.PatientId == q.PatientId.Value);

        if (q.TicketId.HasValue && q.TicketId.Value > 0)
            query = query.Where(d => d.TicketId == q.TicketId.Value);

        if (q.HasRepairCard.HasValue)
            query = q.HasRepairCard.Value
                ? query.Where(d => d.RepairCard != null)
                : query.Where(d => d.RepairCard == null);

        if (q.HasSale.HasValue)
            query = q.HasSale.Value
                ? query.Where(d => d.Sale != null)
                : query.Where(d => d.Sale == null);

        if (q.HasTherapyCards.HasValue)
            query = q.HasTherapyCards.Value
                ? query.Where(d => d.TherapyCard != null)
                : query.Where(d => d.TherapyCard == null);

        if (q.InjuryDateFrom.HasValue)
            query = query.Where(d => d.InjuryDate >= q.InjuryDateFrom.Value);

        if (q.InjuryDateTo.HasValue)
            query = query.Where(d => d.InjuryDate <= q.InjuryDateTo.Value);

        if (q.CreatedDateFrom.HasValue)
        {
            var fromUtc = q.CreatedDateFrom.Value;
            query = query.Where(d => DateOnly.FromDateTime(d.CreatedAtUtc.DateTime) >= fromUtc);
        }

        if (q.CreatedDateTo.HasValue)
        {
            var toUtc = q.CreatedDateTo.Value;
            query = query.Where(d => DateOnly.FromDateTime(d.CreatedAtUtc.DateTime) <= toUtc);
        }

        return query;
    }

    // -------------------- SEARCH --------------------
    private static IQueryable<Diagnosis> ApplySearch(
        IQueryable<Diagnosis> query,
        GetDiagnosesQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var searchTerm = q.SearchTerm;
        var pattern = $"%{searchTerm.Trim().ToLower()}%";

        return query.Where(d =>
            EF.Functions.Like(d.DiagnosisText.ToLower(), pattern) ||
            EF.Functions.Like(d.Id.ToString(), pattern) ||
            (d.Patient != null && d.Patient.Person != null &&
                EF.Functions.Like(d.Patient.Person.FullName.ToLower(), pattern)) ||
            (d.Ticket != null && EF.Functions.Like(d.Ticket.Id.ToString(), pattern)) ||
            d.DiagnosisPrograms.Any(dp =>
                dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern)) ||
            d.DiagnosisIndustrialParts.Any(di =>
                di.IndustrialPartUnit != null &&
                di.IndustrialPartUnit.IndustrialPart != null &&
                EF.Functions.Like(di.IndustrialPartUnit.IndustrialPart.Name.ToLower(), pattern))
        );
    }

    // -------------------- SORTING --------------------
    private static IQueryable<Diagnosis> ApplySorting(
        IQueryable<Diagnosis> query,
        GetDiagnosesQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "createdat";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "createdat"  => isDesc
                ? query.OrderByDescending(d => d.CreatedAtUtc)
                : query.OrderBy(d => d.CreatedAtUtc),

            "injurydate" => isDesc
                ? query.OrderByDescending(d => d.InjuryDate)
                : query.OrderBy(d => d.InjuryDate),

            "type"       => isDesc
                ? query.OrderByDescending(d => d.DiagnoType)
                : query.OrderBy(d => d.DiagnoType),

            "patient"    => isDesc
                ? query.OrderByDescending(d =>
                    d.Patient != null && d.Patient.Person != null
                        ? d.Patient.Person.FullName
                        : string.Empty)
                : query.OrderBy(d =>
                    d.Patient != null && d.Patient.Person != null
                        ? d.Patient.Person.FullName
                        : string.Empty),

            "ticket"     => isDesc
                ? query.OrderByDescending(d => d.Ticket != null ? d.Ticket.Id : 0)
                : query.OrderBy(d => d.Ticket != null ? d.Ticket.Id : 0),

            _            => query.OrderByDescending(d => d.CreatedAtUtc)
        };
    }
}
