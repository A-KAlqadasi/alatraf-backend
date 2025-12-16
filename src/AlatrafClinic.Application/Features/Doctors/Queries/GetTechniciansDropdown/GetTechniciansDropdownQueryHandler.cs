using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using Microsoft.EntityFrameworkCore;
using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public sealed class GetTechniciansDropdownQueryHandler
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
        // Use DateOnly because DiagnosisIndustrialPart.DoctorAssignDate is DateOnly?
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Base query: technicians = doctors in department 2
        // and we project their ACTIVE assignment into TechnicianDto.
        IQueryable<TechnicianDto> techniciansQuery = _context.Doctors
            .AsNoTracking()
            .Where(d => d.DepartmentId == 2)
            .Where(d => d.Assignments.Any(a => a.IsActive))
            .Select(d => new TechnicianDto
            {
                DoctorId = d.Id,
                DoctorName = d.Person!.FullName,

                DoctorSectionRoomId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Id)
                    .FirstOrDefault(),

                SectionId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => (int?)a.SectionId)
                    .FirstOrDefault(),

                SectionName = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Section!.Name)
                    .FirstOrDefault(),

                TodayIndustrialParts = d.Assignments
                    .Where(a => a.IsActive)
                    .SelectMany(a => a.DiagnosisIndustrialParts)
                    .Count(ip => ip.DoctorAssignDate.HasValue && ip.DoctorAssignDate.Value == today)
            });

        techniciansQuery = ApplySectionFilter(techniciansQuery, query);
        techniciansQuery = ApplySearch(techniciansQuery, query);

        var totalCount = await techniciansQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await techniciansQuery
            .OrderBy(t => t.DoctorName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var paged = new PaginatedList<TechnicianDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return paged;
    }

    private static IQueryable<TechnicianDto> ApplySectionFilter(
        IQueryable<TechnicianDto> query,
        GetTechniciansDropdownQuery q)
    {
        if (q.SectionId.HasValue)
        {
            var sectionId = q.SectionId.Value;
            query = query.Where(x => x.SectionId == sectionId);
        }

        return query;
    }

    private static IQueryable<TechnicianDto> ApplySearch(
        IQueryable<TechnicianDto> query,
        GetTechniciansDropdownQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm.Trim().ToLower()}%";

        return query.Where(x =>
            (x.DoctorName != null && EF.Functions.Like(x.DoctorName.ToLower(), pattern)) ||
            (x.SectionName != null && EF.Functions.Like(x.SectionName.ToLower(), pattern))
        );
    }
}
