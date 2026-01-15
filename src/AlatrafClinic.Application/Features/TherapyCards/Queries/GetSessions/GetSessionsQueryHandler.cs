using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sessions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed class GetSessionsQueryHandler
    : IRequestHandler<GetSessionsQuery, Result<PaginatedList<SessionListDto>>>
{
    private readonly IAppDbContext _context;

    public GetSessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<SessionListDto>>> Handle(
        GetSessionsQuery query,
        CancellationToken ct)
    {
        IQueryable<Session> sessionsQuery = _context.Sessions
            .Include(s => s.TherapyCard)
                .ThenInclude(tc => tc.Diagnosis)
                    .ThenInclude(d => d.Patient)
                        .ThenInclude(p => p.Person)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DiagnosisProgram)
                    .ThenInclude(dp => dp.MedicalProgram)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Section)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Room)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Doctor)
                        .ThenInclude(d => d.Person)
            .AsNoTracking();

        sessionsQuery = ApplyFilters(sessionsQuery, query);
        
        if(!string.IsNullOrWhiteSpace(query.SearchTerm))
            sessionsQuery = ApplySearch(sessionsQuery, query);

        sessionsQuery = ApplySorting(sessionsQuery, query);

        var totalCount = await sessionsQuery.CountAsync(ct);
        
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var skip = (page - 1) * pageSize;

        // Apply pagination
        var sessions = await sessionsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // Map results
        var items = sessions.Select(s => new SessionListDto
        {
            SessionId     = s.Id,
            Number        = s.Number,
            IsTaken       = s.IsTaken,
            SessionDate   = s.SessionDate,

            TherapyCardId     = s.TherapyCardId,
            TherapyCardType   = s.TherapyCard!.Type.ToString(),
            ProgramStartDate  = s.TherapyCard.ProgramStartDate,
            ProgramEndDate    = s.TherapyCard.ProgramEndDate,

            PatientId   = s.TherapyCard!.Diagnosis!.PatientId,
            PatientName = s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName,

            SessionPrograms = s.SessionPrograms.ToDtos()

        }).ToList();

        return new PaginatedList<SessionListDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Session> ApplyFilters(
        IQueryable<Session> query,
        GetSessionsQuery q)
    {
        if (q.TherapyCardId.HasValue && q.TherapyCardId.Value > 0)
            query = query.Where(s => s.TherapyCardId == q.TherapyCardId.Value);

        if (q.IsTaken.HasValue)
            query = query.Where(s => s.IsTaken == q.IsTaken.Value);

        if (q.FromDate.HasValue)
            query = query.Where(s => s.SessionDate >= q.FromDate.Value);

        if (q.ToDate.HasValue)
            query = query.Where(s => s.SessionDate <= q.ToDate.Value);

        if (q.DoctorId.HasValue && q.DoctorId.Value > 0)
        {
            var doctorId = q.DoctorId.Value;

            query = query.Where(s =>
                s.SessionPrograms.Any(sp =>
                    sp.DoctorSectionRoom != null &&
                    sp.DoctorSectionRoom.DoctorId == doctorId));
        }

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
        {
            var patientId = q.PatientId.Value;

            query = query.Where(s =>
                s.TherapyCard != null &&
                s.TherapyCard.Diagnosis != null &&
                s.TherapyCard.Diagnosis.PatientId == patientId);
        }

        return query;
    }

    private static IQueryable<Session> ApplySearch(
        IQueryable<Session> query,
        GetSessionsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(s =>
            (s.TherapyCard != null &&
             s.TherapyCard.Diagnosis != null &&
             s.TherapyCard.Diagnosis.Patient != null &&
             s.TherapyCard.Diagnosis.Patient.Person != null &&
             EF.Functions.Like(
                 s.TherapyCard.Diagnosis.Patient.Person.FullName.ToLower(), pattern
             )
            )
            ||
            s.SessionPrograms.Any(sp =>
                sp.DiagnosisProgram != null &&
                sp.DiagnosisProgram.MedicalProgram != null &&
                EF.Functions.Like(
                    sp.DiagnosisProgram.MedicalProgram.Name.ToLower(), pattern
                )
            )
        );
    }

    private static IQueryable<Session> ApplySorting(
        IQueryable<Session> query,
        GetSessionsQuery q)
    {
        var col    = q.SortColumn?.Trim().ToLowerInvariant() ?? "sessiondate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "sessiondate" => isDesc
                ? query.OrderByDescending(s => s.SessionDate)
                : query.OrderBy(s => s.SessionDate),

            "number" => isDesc
                ? query.OrderByDescending(s => s.Number)
                : query.OrderBy(s => s.Number),

            "patient" => isDesc
                ? query.OrderByDescending(s => 
                     s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName
                  )
                : query.OrderBy(s =>
                     s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName
                  ),

            _ => query.OrderByDescending(s => s.SessionDate)
        };
    }
}
