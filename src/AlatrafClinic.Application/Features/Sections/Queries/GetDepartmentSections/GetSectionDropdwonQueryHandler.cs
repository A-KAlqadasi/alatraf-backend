
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetDepartmentSections;

public class GetDepartmentSectionsQueryHandler : IRequestHandler<GetDepartmentSectionsQuery, Result<List<DepartmentSectionDto>>>
{
    private readonly IAppDbContext _context;

    public GetDepartmentSectionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<DepartmentSectionDto>>> Handle(GetDepartmentSectionsQuery query, CancellationToken ct)
    {
        var sections = await _context.Sections
        .Where(s=> s.DepartmentId == query.DepartmentId)
        .Select(s=> new DepartmentSectionDto
        {
            Id = s.Id,
            Name = s.Name,
        })
        .ToListAsync(ct);

        return sections;
    }
}