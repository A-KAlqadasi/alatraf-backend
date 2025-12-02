using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Commands.UpdateSection;

public sealed class UpdateSectionCommandHandler(
    IUnitOfWork unitOfWork,
ICacheService cache,

    ILogger<UpdateSectionCommandHandler> logger
) : IRequestHandler<UpdateSectionCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ICacheService _cache = cache;
  private readonly ILogger<UpdateSectionCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(UpdateSectionCommand request, CancellationToken ct)
  {
    var section = await _unitOfWork.Sections.GetByIdAsync(request.SectionId, ct);
    if (section is null)
    {
      _logger.LogWarning("Section {SectionId} not found.", request.SectionId);
      return ApplicationErrors.SectionNotFound;
    }

    var updateResult = section.UpdateName(request.NewName);
    if (updateResult.IsError)
    {
      _logger.LogWarning("Failed to update Section {SectionId}: {Error}", request.SectionId, updateResult.Errors);
      return updateResult.Errors;
    }

    await _unitOfWork.Sections.UpdateAsync(section, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation(" Section {SectionId} renamed successfully to '{NewName}'.",
        section.Id, section.Name);
    await _cache.RemoveByTagAsync("section", ct);

    return Result.Updated;
  }
}