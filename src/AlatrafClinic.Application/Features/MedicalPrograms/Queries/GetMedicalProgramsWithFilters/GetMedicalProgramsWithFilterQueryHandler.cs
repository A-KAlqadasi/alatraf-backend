using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.MedicalPrograms;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramsWithFilters;

public sealed class GetMedicalProgramsWithFilterQueryHandler
    : IRequestHandler<GetMedicalProgramsWithFilterQuery, Result<PaginatedList<MedicalProgramDto>>>
{
    private readonly IAppDbContext _context;

    public GetMedicalProgramsWithFilterQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<MedicalProgramDto>>> Handle(
        GetMedicalProgramsWithFilterQuery query,
        CancellationToken ct)
    {
        IQueryable<MedicalProgram> programsQuery = _context.MedicalPrograms
            .Include(mp => mp.Section)
            .AsNoTracking();

        programsQuery = ApplyFilters(programsQuery, query);
        programsQuery = ApplySearch(programsQuery, query);
        programsQuery = ApplySorting(programsQuery, query);

        var totalCount = await programsQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await programsQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(mp => new MedicalProgramDto
            {
                Id = mp.Id,
                Name = mp.Name,
                Description = mp.Description,
                SectionId = mp.SectionId,
                SectionName = mp.Section != null ? mp.Section.Name : null
            })
            .ToListAsync(ct);

        return new PaginatedList<MedicalProgramDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<MedicalProgram> ApplyFilters(
        IQueryable<MedicalProgram> query,
        GetMedicalProgramsWithFilterQuery q)
    {
        if (q.SectionId.HasValue && q.SectionId.Value > 0)
        {
            var sectionId = q.SectionId.Value;
            query = query.Where(mp => mp.SectionId == sectionId);
        }

        if (q.HasSection.HasValue)
        {
            query = q.HasSection.Value
                ? query.Where(mp => mp.SectionId != null)
                : query.Where(mp => mp.SectionId == null);
        }

        return query;
    }

    private static IQueryable<MedicalProgram> ApplySearch(
        IQueryable<MedicalProgram> query,
        GetMedicalProgramsWithFilterQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term = q.SearchTerm.Trim().ToLowerInvariant();
        var pattern = $"%{term}%";

        return query.Where(mp =>
            EF.Functions.Like(mp.Name.ToLower(), pattern) ||
            (mp.Description != null && EF.Functions.Like(mp.Description.ToLower(), pattern)) ||
            (mp.Section != null && EF.Functions.Like(mp.Section.Name.ToLower(), pattern)));
    }

    private static IQueryable<MedicalProgram> ApplySorting(
        IQueryable<MedicalProgram> query,
        GetMedicalProgramsWithFilterQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "name" => isDesc
                ? query.OrderByDescending(mp => mp.Name)
                : query.OrderBy(mp => mp.Name),

            "section" or "sectionname" => isDesc
                ? query.OrderByDescending(mp => mp.Section != null ? mp.Section.Name : string.Empty)
                : query.OrderBy(mp => mp.Section != null ? mp.Section.Name : string.Empty),

            "createdat" => isDesc ? query.OrderByDescending(mp => mp.CreatedAtUtc) : query.OrderBy(mp => mp.CreatedAtUtc),

            _ => isDesc
                ? query.OrderByDescending(mp => mp.Name)
                : query.OrderBy(mp => mp.Name)
        };
    }
}
