using MediatR;

using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public sealed class GetTherapyDiagnosesQueryHandler
    : IRequestHandler<GetTherapyDiagnosesQuery, Result<PaginatedList<TherapyCardDiagnosisDto>>>
{
    private readonly IAppDbContext _context;

    public GetTherapyDiagnosesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TherapyCardDiagnosisDto>>> Handle(
        GetTherapyDiagnosesQuery query,
        CancellationToken ct)
    {
        // Base query + includes (readable, consistent with your production style)
        IQueryable<Diagnosis> diagnosesQuery = _context.Diagnoses
            .Include(d => d.Patient)
                .ThenInclude(p => p.Person)
            .Include(d => d.TherapyCard)
            .Include(d => d.Payments)
            .Include(d => d.InjuryReasons)
            .Include(d => d.InjurySides)
            .Include(d => d.InjuryTypes)
            .Include(d => d.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .AsNoTracking();

        // Apply filters, search, sorting
        diagnosesQuery = ApplyFilters(diagnosesQuery);
        diagnosesQuery = ApplySearch(diagnosesQuery, query);
        diagnosesQuery = ApplySorting(diagnosesQuery, query);

        var totalCount = await diagnosesQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        // Paging
        var diagnoses = await diagnosesQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // Mapping
        var items = diagnoses
            .Select(MapToDto)
            .ToList();

        return new PaginatedList<TherapyCardDiagnosisDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Diagnosis> ApplyFilters(IQueryable<Diagnosis> query)
    {
        // Therapy diagnoses only
        query = query.Where(d => d.DiagnoType == DiagnosisType.Therapy);

        // Must have a TherapyCard
        query = query.Where(d => d.TherapyCard != null);

        // Must be paid: completed + fully paid + payment date exists
        query = query.Where(d =>
            d.Payments.Any(p => p.IsCompleted && p.PaymentDate != null));

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<Diagnosis> ApplySearch(
        IQueryable<Diagnosis> query,
        GetTherapyDiagnosesQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term = q.SearchTerm.Trim();

        // If number => search by TherapyCardId
        if (int.TryParse(term, out var therapyCardId))
        {
            return query.Where(d => d.TherapyCard!.Id == therapyCardId);
        }

        // Else => search by PatientName (LIKE)
        var pattern = $"%{term.ToLower()}%";

        return query.Where(d =>
            d.Patient != null &&
            d.Patient.Person != null &&
            EF.Functions.Like(d.Patient.Person.FullName.ToLower(), pattern));
    }

    // ---------------- SORTING ----------------
    private static IQueryable<Diagnosis> ApplySorting(
        IQueryable<Diagnosis> query,
        GetTherapyDiagnosesQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "paymentdate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        // Order by earliest fully-paid payment date per diagnosis
        // (This matches your business requirement precisely)
        return col switch
        {
            "paymentdate" => isDesc
                ? query.OrderByDescending(d => d.Payments
                    .Where(p => p.IsCompleted && p.PaymentDate != null)
                    .Min(p => p.PaymentDate))
                : query.OrderBy(d => d.Payments
                    .Where(p => p.IsCompleted && p.PaymentDate != null)
                    .Min(p => p.PaymentDate)),

            "therapycardid" => isDesc
                ? query.OrderByDescending(d => d.TherapyCard!.Id)
                : query.OrderBy(d => d.TherapyCard!.Id),

            "patient" => isDesc
                ? query.OrderByDescending(d => d.Patient!.Person!.FullName)
                : query.OrderBy(d => d.Patient!.Person!.FullName),

            _ => query.OrderBy(d => d.Payments
                .Where(p => p.IsCompleted && p.PaymentDate != null)
                .Min(p => p.PaymentDate)),
        };
    }

    // ---------------- MAPPING ----------------
    private static TherapyCardDiagnosisDto MapToDto(Diagnosis d)
    {
        var person = d.Patient.Person;

        return new TherapyCardDiagnosisDto
        {
            TicketId = d.TicketId,
            PatientId = d.PatientId,
            PatientName = person.FullName,
            Gender = person.Gender ? "Male" : "Female",
            Age = CalculateAge(person.Birthdate, AlatrafClinicConstants.TodayDate),

            DiagnosisId = d.Id,
            DiagnosisText = d.DiagnosisText,
            InjuryDate = d.InjuryDate,
            DiagnosisType = d.DiagnoType.ToString(),

            InjuryReasons = d.InjuryReasons
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            InjurySides = d.InjurySides
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            InjuryTypes = d.InjuryTypes
                .Select(x => new InjuryDto { Id = x.Id, Name = x.Name })
                .ToList(),

            Programs = d.DiagnosisPrograms
                .Select(dp => new DiagnosisProgramDto
                {
                    DiagnosisProgramId = dp.Id,
                    MedicalProgramId = dp.MedicalProgramId,
                    ProgramName = dp.MedicalProgram!.Name,
                    Duration = dp.Duration,
                    Notes = dp.Notes
                })
                .ToList(),

            TherapyCardId = d.TherapyCard!.Id,
            ProgramStartDate = d.TherapyCard.ProgramStartDate,
            ProgramEndDate = d.TherapyCard.ProgramEndDate,
            TherapyCardType = d.TherapyCard.Type.ToArabicTherapyCardType(),
            CardStatus = d.TherapyCard.CardStatus.ToArabicTherapyCardStatus(),
            Notes = d.TherapyCard.Notes
        };
    }

    private static int CalculateAge(DateOnly birthdate, DateOnly today)
    {
        var age = today.Year - birthdate.Year;
        if (birthdate > today.AddYears(-age)) age--;
        return age;
    }
}