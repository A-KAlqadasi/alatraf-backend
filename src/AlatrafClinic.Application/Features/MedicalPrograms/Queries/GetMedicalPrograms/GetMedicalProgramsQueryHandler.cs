using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public class GetMedicalProgramsQueryHandler
    : IRequestHandler<GetMedicalProgramsQuery, Result<PaginatedList<MedicalProgramDto>>>
{
    private readonly IAppDbContext _context;

    public GetMedicalProgramsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<MedicalProgramDto>>> Handle(
        GetMedicalProgramsQuery query,
        CancellationToken ct)
    {
        IQueryable<MedicalProgram> programsQuery = _context.MedicalPrograms
            .AsNoTracking();

        programsQuery = ApplySearch(programsQuery, query);

        var totalCount = await programsQuery.CountAsync(ct);

        if (totalCount == 0)
        {
            return Error.NotFound(description: "Medical programs not found.");
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var programs = await programsQuery
            .OrderBy(mp => mp.Name)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = programs
            .ToDtos()
            .ToList();

        var paged = new PaginatedList<MedicalProgramDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return paged;
    }

    private static IQueryable<MedicalProgram> ApplySearch(
        IQueryable<MedicalProgram> query,
        GetMedicalProgramsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(mp =>
            EF.Functions.Like(mp.Name.ToLower(), pattern) ||
            (mp.Description != null &&
             EF.Functions.Like(mp.Description.ToLower(), pattern))
        );
    }
}