using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Sessions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed class GetSessionsQueryHandler
    : IRequestHandler<GetSessionsQuery, Result<PaginatedList<SessionListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSessionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<SessionListDto>>> Handle(GetSessionsQuery query, CancellationToken ct)
    {
        var sessionsQuery = await _unitOfWork.Sessions.GetSessionsQueryAsync();

        sessionsQuery = ApplyFilters(sessionsQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            sessionsQuery = ApplySearch(sessionsQuery, query.SearchTerm!);

        sessionsQuery = ApplySorting(sessionsQuery, query.SortColumn, query.SortDirection);

        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await sessionsQuery.CountAsync(ct);

        var items = await sessionsQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(s => new SessionListDto
            {
                SessionId = s.Id,
                Number = s.Number,
                IsTaken = s.IsTaken,
                SessionDate = s.SessionDate,

                TherapyCardId = s.TherapyCardId,
                TherapyCardType = s.TherapyCard!.Type.ToString(),
                ProgramStartDate = s.TherapyCard.ProgramStartDate,
                ProgramEndDate = s.TherapyCard.ProgramEndDate,

                PatientId = s.TherapyCard!.Diagnosis!.PatientId,
                PatientName = s.TherapyCard.Diagnosis.Patient!.Person!.FullName,

                SessionPrograms = s.SessionPrograms.Select(sp => new SessionProgramDto
                {
                    SessionProgramId = sp.Id,
                    DiagnosisProgramId = sp.DiagnosisProgramId,
                    ProgramName = sp.DiagnosisProgram!.MedicalProgram!.Name,
                    DoctorSectionRoomId = sp.DoctorSectionRoomId,
                    DoctorSectionRoomName = sp.DoctorSectionRoom!.Section!.Name + " - " + sp.DoctorSectionRoom!.Room!.Name.ToString(),
                    DoctorName = sp.DoctorSectionRoom!.Doctor!.Person!.FullName
                }).ToList()
            })
            .ToListAsync(ct);

        return new PaginatedList<SessionListDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Session> ApplyFilters(IQueryable<Session> query, GetSessionsQuery q)
    {
        if (q.TherapyCardId.HasValue && q.TherapyCardId > 0)
            query = query.Where(s => s.TherapyCardId == q.TherapyCardId);

        if (q.IsTaken.HasValue)
            query = query.Where(s => s.IsTaken == q.IsTaken.Value);

        if (q.FromDate.HasValue)
            query = query.Where(s => s.SessionDate >= q.FromDate.Value);

        if (q.ToDate.HasValue)
            query = query.Where(s => s.SessionDate <= q.ToDate.Value);

        if (q.DoctorId.HasValue && q.DoctorId > 0)
            query = query.Where(s =>
                s.SessionPrograms.Any(sp =>
                    sp.DoctorSectionRoom != null &&
                    sp.DoctorSectionRoom.DoctorId == q.DoctorId));

        if (q.PatientId.HasValue && q.PatientId > 0)
            query = query.Where(s =>
                s.TherapyCard != null &&
                s.TherapyCard.Diagnosis != null &&
                s.TherapyCard.Diagnosis.PatientId == q.PatientId);

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<Session> ApplySearch(IQueryable<Session> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(s =>
            (s.TherapyCard != null && s.TherapyCard.Diagnosis != null &&
             s.TherapyCard.Diagnosis.Patient != null &&
             s.TherapyCard.Diagnosis.Patient.Person != null &&
             EF.Functions.Like(s.TherapyCard.Diagnosis.Patient.Person.FullName.ToLower(), pattern))
            ||
            s.SessionPrograms.Any(sp =>
                sp.DiagnosisProgram != null &&
                sp.DiagnosisProgram.MedicalProgram != null &&
                EF.Functions.Like(sp.DiagnosisProgram.MedicalProgram.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private static IQueryable<Session> ApplySorting(IQueryable<Session> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "sessiondate";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "sessiondate" => isDesc
                ? query.OrderByDescending(s => s.SessionDate)
                : query.OrderBy(s => s.SessionDate),

            "number" => isDesc
                ? query.OrderByDescending(s => s.Number)
                : query.OrderBy(s => s.Number),

            "patient" => isDesc
                ? query.OrderByDescending(s => s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(s => s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(s => s.SessionDate)
        };
    }
}