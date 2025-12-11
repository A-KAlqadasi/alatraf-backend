using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public sealed class GetTherapyCardsQueryHandler
    : IRequestHandler<GetTherapyCardsQuery, Result<PaginatedList<TherapyCardDto>>>
{
    private readonly IAppDbContext _context;

    public GetTherapyCardsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TherapyCardDto>>> Handle(
        GetTherapyCardsQuery query,
        CancellationToken ct)
    {
        IQueryable<TherapyCard> cardsQuery = _context.TherapyCards
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjuryTypes)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc=> tc.InjurySides)
            .Include(tc => tc.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .Include(tc => tc.Sessions)
                .ThenInclude(tc=> tc.SessionPrograms)
                    .ThenInclude(tc=> tc.DiagnosisProgram)
                        .ThenInclude(tc=> tc.MedicalProgram)
            .AsNoTracking();

        cardsQuery = ApplyFilters(cardsQuery, query);

        if(!string.IsNullOrWhiteSpace(query.SearchTerm))
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

        var items = cards
            .Select(tc => tc.ToDto())
            .ToList();

        return new PaginatedList<TherapyCardDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<TherapyCard> ApplyFilters(
        IQueryable<TherapyCard> query,
        GetTherapyCardsQuery q)
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

        if (q.DiagnosisId.HasValue && q.DiagnosisId.Value > 0)
            query = query.Where(tc => tc.DiagnosisId == q.DiagnosisId.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(tc =>
                tc.Diagnosis != null &&
                tc.Diagnosis.PatientId == q.PatientId.Value);

        return query;
    }

    private static IQueryable<TherapyCard> ApplySearch(
        IQueryable<TherapyCard> query,
        GetTherapyCardsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(tc =>
            (tc.Diagnosis != null &&
                (EF.Functions.Like(tc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (tc.Diagnosis.Patient != null &&
                  tc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(tc.Diagnosis.Patient.Person.FullName.ToLower(), pattern))))
            ||
            tc.DiagnosisPrograms.Any(dp =>
                dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern))
        );
    }

    private static IQueryable<TherapyCard> ApplySorting(
        IQueryable<TherapyCard> query,
        GetTherapyCardsQuery q)
    {
        var col    = q.SortColumn?.Trim().ToLowerInvariant() ?? "programstartdate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
