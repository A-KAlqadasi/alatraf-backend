using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;


namespace AlatrafClinic.Application.Features.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandHandler(
    IAppDbContext _context,
    HybridCache _cache,
    ILogger<CreateSectionCommandHandler> _logger
) : IRequestHandler<CreateSectionCommand, Result<SectionDto>>
{


    public async Task<Result<SectionDto>> Handle(CreateSectionCommand command, CancellationToken ct)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.DepartmentId, ct);

        if (department is null)
        {
            _logger.LogWarning("Department {DepartmentId} not found when creating sections.", command.DepartmentId);
            return ApplicationErrors.DepartmentNotFound;
        }
        var isExists = await _context.Sections.AnyAsync(s=> s.Name == command.Name, ct);
        if (isExists)
        {
            _logger.LogWarning("Section with name {Name} already exists.", command.Name);
            return SectionErrors.DuplicateSectionName;
        }

        var createResult = Section.Create(command.Name, department.Id);
        if (createResult.IsError)
        {
            return createResult.Errors;
        }
        var section = createResult.Value;


        await _context.Sections.AddAsync(section, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("section", ct);

        _logger.LogInformation("Section {SectionName} created successfully for Department {DepartmentId}.",
            section.Name, command.DepartmentId);

        return section.ToDto();
    }
}