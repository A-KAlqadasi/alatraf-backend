using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

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

    var department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.DepartmentId, ct);

    if (department is null)
    {
        _logger.LogWarning("Department {DepartmentId} not found when update sections.", command.DepartmentId);
        return ApplicationErrors.DepartmentNotFound;
    }

    var updateResult = section.UpdateName(command.NewName);
    if (updateResult.IsError)
    {
        _logger.LogWarning("Failed to update Section {SectionId}: {Error}", command.SectionId, updateResult.Errors);
        return updateResult.Errors;
    }
    section.Department = department;

    _context.Sections.Update(section);
    await _context.SaveChangesAsync(ct);

    _logger.LogInformation(" Section {SectionId} renamed successfully to '{NewName}'.",
        section.Id, section.Name);
    await _cache.RemoveByTagAsync("section", ct);

    return Result.Updated;
  }
}