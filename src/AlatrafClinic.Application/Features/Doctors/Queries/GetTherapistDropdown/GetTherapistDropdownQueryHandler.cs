using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;
public sealed class GetTherapistDropdownQueryHandler
    : IRequestHandler<GetTherapistDropdownQuery, Result<PaginatedList<TherapistDto>>>
{
    private readonly IAppDbContext _context;

    public GetTherapistDropdownQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TherapistDto>>> Handle(
        GetTherapistDropdownQuery query,
        CancellationToken ct)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var tomorrowUtc = todayUtc.AddDays(1);

        // Base query: doctors in Therapy department (1) with their ACTIVE assignment projected
        IQueryable<TherapistDto> therapistsQuery = _context.Doctors
            .AsNoTracking()
            .Where(d => d.DepartmentId == 1 )
            .Where(d => d.Assignments.Any(a => a.IsActive))
            .Select(d => new TherapistDto
            {
                DoctorId = d.Id,
                DoctorName = d.Person!.FullName,

                // Active assignment fields (nullable-friendly)
                DoctorSectionRoomId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => (int?)a.Id)
                    .SingleOrDefault(),

                SectionId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => (int?)a.SectionId)
                    .SingleOrDefault(),

                SectionName = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Section.Name)
                    .SingleOrDefault(),

                RoomName = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Room!.Name)
                    .SingleOrDefault(),

                RoomId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => (int?)a.RoomId!)
                    .SingleOrDefault(),

                TodaySessions = d.Assignments
                    .Where(a => a.IsActive)
                    .SelectMany(a => a.SessionPrograms)
                    .Count(sp => sp.CreatedAtUtc >= todayUtc && sp.CreatedAtUtc < tomorrowUtc),
            });

        // Apply filters on the projected dto (still SQL-translatable)
        therapistsQuery = ApplySectionFilter(therapistsQuery, query);
        therapistsQuery = ApplySearch(therapistsQuery, query);

        var totalCount = await therapistsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await therapistsQuery
            .OrderBy(t => t.DoctorName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var paged = new PaginatedList<TherapistDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return paged;
    }

    private static IQueryable<TherapistDto> ApplySectionFilter(
        IQueryable<TherapistDto> query,
        GetTherapistDropdownQuery q)
    {
        if (q.SectionId.HasValue)
            query = query.Where(x => x.SectionId == q.SectionId.Value);

        if (q.RoomId.HasValue)
            query = query.Where(x => x.RoomId == q.RoomId.Value);

        return query;
    }

    private static IQueryable<TherapistDto> ApplySearch(
        IQueryable<TherapistDto> query,
        GetTherapistDropdownQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term = q.SearchTerm.Trim().ToLower();
        var pattern = $"%{term}%";

        return query.Where(x =>
            (x.DoctorName != null && EF.Functions.Like(x.DoctorName.ToLower(), pattern)) ||
            (x.SectionName != null && EF.Functions.Like(x.SectionName.ToLower(), pattern)) ||
            (x.RoomName != null && EF.Functions.Like(x.RoomName.ToLower(), pattern))
        );
    }
}
