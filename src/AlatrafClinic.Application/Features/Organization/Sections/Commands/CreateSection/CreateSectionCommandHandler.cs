using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Sections.Dtos;
using AlatrafClinic.Application.Features.Organization.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.Sections;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;


namespace AlatrafClinic.Application.Features.Organization.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandHandler(
    IUnitOfWork unitOfWork,
ICacheService cache,
    ILogger<CreateSectionCommandHandler> logger
) : IRequestHandler<CreateSectionCommand, Result<List<SectionDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;
    private readonly ILogger<CreateSectionCommandHandler> _logger = logger;

    public async Task<Result<List<SectionDto>>> Handle(CreateSectionCommand request, CancellationToken ct)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, ct);
        if (department is null)
        {
            _logger.LogWarning("❌ Department {DepartmentId} not found when creating sections.", request.DepartmentId);
            return ApplicationErrors.DepartmentNotFound;
        }

        var createdSections = new List<Section>();

    foreach (var name in request.SectionNames)
    {
      var sectionName = name.Trim();

      var result = department.AddSection(sectionName);
      if (result.IsError)
        return result.Errors;

      createdSections.Add(result.Value);
    }

    // await _unitOfWork.Departments.UpdateAsync(department, ct); // Use this to fallow the DDD 
    await _unitOfWork.Sections.AddRangeAsync(createdSections, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(" {Count} section(s) created successfully for Department {DepartmentId}.",
            createdSections.Count, request.DepartmentId);

    await _cache.RemoveByTagAsync("section", ct);
    return createdSections.ToDtos();
    }
}