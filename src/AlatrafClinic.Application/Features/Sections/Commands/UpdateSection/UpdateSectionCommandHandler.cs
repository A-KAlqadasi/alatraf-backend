using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Commands.UpdateSection;

public sealed class UpdateSectionCommandHandler(
    IAppDbContext _context,
    HybridCache _cache,
    ILogger<UpdateSectionCommandHandler> _logger
) : IRequestHandler<UpdateSectionCommand, Result<Updated>>
{

    public async Task<Result<Updated>> Handle(UpdateSectionCommand command, CancellationToken ct)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(s=> s.Id == command.SectionId, ct);
        
        if (section is null)
        {
            _logger.LogWarning("Section {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        var isExists = await _context.Sections.AnyAsync(s=> s.Name == command.NewName, ct);
        if (isExists && section.Name != command.NewName)
        {
            _logger.LogWarning("Section with name {Name} already exists.", command.NewName);
            return SectionErrors.DuplicateSectionName;
        }

        var updateResult = section.UpdateName(command.NewName);
        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update Section {SectionId}: {Error}", command.SectionId, updateResult.Errors);
            return updateResult.Errors;
        }

        _context.Sections.Update(section);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("section", ct);

        _logger.LogInformation(" Section {SectionId} renamed successfully to '{NewName}'.",
            section.Id, section.Name);

        return Result.Updated;
    }
}