using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;


namespace AlatrafClinic.Application.Features.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandHandler(
    IUnitOfWork unitOfWork,
ICacheService cache,
    ILogger<CreateSectionCommandHandler> logger
) : IRequestHandler<CreateSectionCommand, Result<SectionDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;
    private readonly ILogger<CreateSectionCommandHandler> _logger = logger;

    public async Task<Result<SectionDto>> Handle(CreateSectionCommand command, CancellationToken ct)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(command.DepartmentId, ct);
        if (department is null)
        {
            _logger.LogWarning("Department {DepartmentId} not found when creating sections.", command.DepartmentId);
            return ApplicationErrors.DepartmentNotFound;
        }
        var isExists = await _unitOfWork.Departments.IsExistAsync(command.Name, ct);
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


        await _unitOfWork.Sections.AddAsync(section, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Section {SectionName} created successfully for Department {DepartmentId}.",
            section.Name, command.DepartmentId);

        await _cache.RemoveByTagAsync("section", ct);

        return section.ToDto();
    }
}