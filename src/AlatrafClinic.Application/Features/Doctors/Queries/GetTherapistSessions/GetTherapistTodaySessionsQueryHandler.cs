using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistSessions;

public sealed class GetTherapistSessionsQueryHandler
    : IRequestHandler<GetTherapistSessionsQuery, Result<PaginatedList<TherapistSessionProgramDto>>>
{
    private readonly IAppDbContext _context;

    public GetTherapistSessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TherapistSessionProgramDto>>> Handle(
        GetTherapistSessionsQuery query,
        CancellationToken ct)
    {

        IQueryable<TherapistSessionProgramDto> sessionsQuery = _context.SessionPrograms
            .AsNoTracking()
            .Where(sp => sp.DoctorSectionRoomId == query.DoctorSectionRoomId)

            .Select(sp => new TherapistSessionProgramDto
            {
                SessionProgramId = sp.Id,
                DiagnosisProgramId = sp.DiagnosisProgramId,
                ProgramName = sp.DiagnosisProgram.MedicalProgram!.Name,
                SessionId = sp.SessionId,
                SessionNumber = sp.Session!.Number,
                SessionDate = sp.Session.SessionDate, // Added for filtering
                TherapyCardId = sp.Session!.TherapyCardId,
                PatientName = sp.Session!.TherapyCard!.Diagnosis.Patient!.Person!.FullName,
                PatientPhoneNumber = sp.Session!.TherapyCard!.Diagnosis.Patient!.Person.Phone
            });

        // 2. Apply all optional filters
        sessionsQuery = ApplyFilters(sessionsQuery, query);

        // 3. Manually handle pagination
        var totalCount = await sessionsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await sessionsQuery
            .OrderByDescending(s => s.SessionDate) // Order by date for consistent paging
            .ThenByDescending(s => s.SessionProgramId)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // 4. Construct the final PaginatedList object
        var pagedResult = new PaginatedList<TherapistSessionProgramDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return pagedResult;
    }

    private static IQueryable<TherapistSessionProgramDto> ApplyFilters(
        IQueryable<TherapistSessionProgramDto> query,
        GetTherapistSessionsQuery q)
    {
        // Use the date from the query, or default to today
        var filterDate = q.Date ?? AlatrafClinicConstants.TodayDate;
        query = query.Where(s => s.SessionDate == filterDate);

        // Filter by TherapyCard ID
        if (q.TherapyCardId.HasValue)
        {
            query = query.Where(x => x.TherapyCardId == q.TherapyCardId.Value);
        }

        // Filter by Patient Name (as a "contains" search)
        if (!string.IsNullOrWhiteSpace(q.PatientName))
        {
            var searchTerm = q.PatientName.Trim();
            query = query.Where(x => x.PatientName.Contains(searchTerm));
        }

        return query;
    }
}
