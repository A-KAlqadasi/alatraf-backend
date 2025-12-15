using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSectionById;

public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionDto>>
{
    private readonly ILogger<GetSectionByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetSectionByIdQueryHandler(ILogger<GetSectionByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<SectionDto>> Handle(GetSectionByIdQuery query, CancellationToken ct)
    {
        var section = await _context.Sections
        .Include(s=> s.Rooms)
        .Include(s=> s.Department)
        .FirstOrDefaultAsync(s=> s.Id == query.SectionId, ct);
        
        if (section is null)
        {
            _logger.LogWarning("Section with ID {SectionId} not found.", query.SectionId);
            return SectionErrors.SectionNotFound;
        }

        return section.ToDto();
    }
}