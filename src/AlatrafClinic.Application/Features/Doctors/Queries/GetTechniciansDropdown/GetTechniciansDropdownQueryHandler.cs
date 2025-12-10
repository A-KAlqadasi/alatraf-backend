using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public class GetTechniciansDropdownQueryHandler
    : IRequestHandler<GetTechniciansDropdownQuery, Result<PaginatedList<TechnicianDto>>>
{
    private readonly IAppDbContext _context;

    public GetTechniciansDropdownQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<PaginatedList<TechnicianDto>>> Handle(
        GetTechniciansDropdownQuery query,
        CancellationToken ct)
    {
        IQueryable<DoctorSectionRoom> techniciansQuery = _context.DoctorSectionRooms
            .Include(dsrm => dsrm.DiagnosisIndustrialParts)
            .Include(dsrm => dsrm.Doctor)
                .ThenInclude(d => d.Person)
            .Include(dsrm => dsrm.Section)
            .AsNoTracking()
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodayIndustrialPartsCount() > 0);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            techniciansQuery = ApplySearch(techniciansQuery, query);
        }

        var totalCount = await techniciansQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        
        var skip = (page - 1) * pageSize;

        var technicianAssignments = await techniciansQuery
            .OrderBy(dsrm => dsrm.Doctor!.Person!.FullName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = technicianAssignments
            .ToTechnicianDtos()
            .ToList();

        var paged = new PaginatedList<TechnicianDto>
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
        GetTechniciansDropdownQuery q)
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
        );
    }
}