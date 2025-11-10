
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public class GetTherapyCardsQueryHandler
    : IRequestHandler<GetTherapyCardsQuery, Result<PaginatedList<TherapyCardDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapyCardsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<TherapyCardDto>>> Handle(GetTherapyCardsQuery query, CancellationToken ct)
    {
        var therapyQuery = await _unitOfWork.TherapyCards.GetTherapyCardsQueryAsync();

        therapyQuery = ApplyFilters(therapyQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            therapyQuery = ApplySearch(therapyQuery, query.SearchTerm!);

        therapyQuery = ApplySorting(therapyQuery, query.SortColumn, query.SortDirection);

        // Paging guards
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await therapyQuery.CountAsync(ct);

        var items = await therapyQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(tc => new TherapyCardDto
            {
                TherapyCardId = tc.Id,
                IsActive = tc.IsActive,
                NumberOfSessions = tc.NumberOfSessions,
                ProgramStartDate = tc.ProgramStartDate,
                ProgramEndDate = tc.ProgramEndDate,
                TherapyCardType = tc.Type,
                CardStatus = tc.CardStatus,
                Notes = null, // optional: you can add notes later if stored
                Diagnosis = tc.Diagnosis != null ? tc.Diagnosis.ToDto() : new DiagnosisDto(),
                Programs = tc.DiagnosisPrograms
                    .Select(dp => new DiagnosisProgramDto
                    {
                        DiagnosisProgramId = dp.Id,
                        ProgramName = dp.MedicalProgram != null ? dp.MedicalProgram.Name : string.Empty,
                        MedicalProgramId = dp.MedicalProgramId,
                        Duration = dp.Duration,
                        Notes = dp.Notes
                    }).ToList()
            })
            .ToListAsync(ct);

        return new PaginatedList<TherapyCardDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<TherapyCard> ApplyFilters(IQueryable<TherapyCard> query, GetTherapyCardsQuery q)
    {
        if (q.IsActive.HasValue)
            query = query.Where(tc => tc.IsActive == q.IsActive.Value);

        if (q.Type.HasValue)
            query = query.Where(tc => tc.Type == q.Type.Value);

        if (q.Status.HasValue)
            query = query.Where(tc => tc.CardStatus == q.Status.Value);

        if (q.ProgramStartFrom.HasValue)
            query = query.Where(tc => tc.ProgramStartDate >= q.ProgramStartFrom.Value);

        if (q.ProgramStartTo.HasValue)
            query = query.Where(tc => tc.ProgramStartDate <= q.ProgramStartTo.Value);

        if (q.ProgramEndFrom.HasValue)
            query = query.Where(tc => tc.ProgramEndDate >= q.ProgramEndFrom.Value);

        if (q.ProgramEndTo.HasValue)
            query = query.Where(tc => tc.ProgramEndDate <= q.ProgramEndTo.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId > 0)
            query = query.Where(tc => tc.DiagnosisId == q.DiagnosisId);

        if (q.PatientId.HasValue && q.PatientId > 0)
            query = query.Where(tc => tc.Diagnosis != null && tc.Diagnosis.PatientId == q.PatientId);

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<TherapyCard> ApplySearch(IQueryable<TherapyCard> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(tc =>
            (tc.Diagnosis != null &&
                (EF.Functions.Like(tc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (tc.Diagnosis.Patient != null && tc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(tc.Diagnosis.Patient.Person.FullName.ToLower(), pattern)))) ||
            tc.DiagnosisPrograms.Any(dp => dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private static IQueryable<TherapyCard> ApplySorting(IQueryable<TherapyCard> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "programstartdate";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "programstartdate" => isDesc
                ? query.OrderByDescending(tc => tc.ProgramStartDate)
                : query.OrderBy(tc => tc.ProgramStartDate),

            "programenddate" => isDesc
                ? query.OrderByDescending(tc => tc.ProgramEndDate)
                : query.OrderBy(tc => tc.ProgramEndDate),

            "type" => isDesc
                ? query.OrderByDescending(tc => tc.Type)
                : query.OrderBy(tc => tc.Type),

            "status" => isDesc
                ? query.OrderByDescending(tc => tc.CardStatus)
                : query.OrderBy(tc => tc.CardStatus),

            "patient" => isDesc
                ? query.OrderByDescending(tc => tc.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(tc => tc.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(tc => tc.ProgramStartDate)
        };
    }
}