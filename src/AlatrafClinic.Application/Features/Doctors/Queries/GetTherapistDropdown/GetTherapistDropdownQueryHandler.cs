using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public class GetTherapistDropdownQueryHandler
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
        IQueryable<DoctorSectionRoom> therapistsQuery = _context.DoctorSectionRooms
            .Include(dsrm => dsrm.Doctor)
                .ThenInclude(d => d.Person)
            .Include(dsrm => dsrm.Section)
                .ThenInclude(s => s.Rooms)
            .AsNoTracking()
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodaySessionsCount() > 0);

        therapistsQuery = ApplySearch(therapistsQuery, query);

        var totalCount = await therapistsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var therapistAssignments = await therapistsQuery
            .OrderBy(dsrm => dsrm.Doctor!.Person!.FullName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = therapistAssignments
            .ToTherapistDtos()
            .ToList();

        var paged = new PaginatedList<TherapistDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return paged;
    }

    private static IQueryable<DoctorSectionRoom> ApplySearch(
        IQueryable<DoctorSectionRoom> query,
        GetTherapistDropdownQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(dsrm =>
            (dsrm.Doctor != null &&
             dsrm.Doctor.Person != null &&
             EF.Functions.Like(dsrm.Doctor.Person.FullName.ToLower(), pattern))
            ||
            (dsrm.Section != null &&
             EF.Functions.Like(dsrm.Section.Name.ToLower(), pattern))
            || 
            (dsrm.Room != null &&
             EF.Functions.Like(dsrm.Room.Name.ToLower(), pattern))
        );
    }
}