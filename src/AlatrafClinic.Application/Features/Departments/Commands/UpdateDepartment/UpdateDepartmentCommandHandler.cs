using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Departments.Commands.UpdateDepartment;

public sealed class UpdateDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateDepartmentCommandHandler> logger,

HybridCache cache
) : IRequestHandler<UpdateDepartmentCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<UpdateDepartmentCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(UpdateDepartmentCommand command, CancellationToken ct)
  {
    var name = command.NewName.Trim();

    var department = await _unitOfWork.Departments.GetByIdAsync(command.DepartmentId, ct);
    if (department is null)
    {
      _logger.LogWarning(" Department {DepartmentId} not found.", command.DepartmentId);
      return ApplicationErrors.DepartmentNotFound;
    }

    var existing = await _unitOfWork.Departments.IsExistAsync(name, ct);

    if (existing && department.Name.Trim() != command.NewName.Trim())
    {
      _logger.LogWarning("Another department with name '{DepartmentName}' already exists.", name);
      return ApplicationErrors.DepartmentAlreadyExists(name);
    }

    var renameResult = department.Update(name);
    if (renameResult.IsError)
      return renameResult.Errors;

    await _unitOfWork.Departments.UpdateAsync(department, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation("Department {DepartmentId} renamed successfully to '{DepartmentName}'.",
        department.Id, department.Name);
    await _cache.RemoveByTagAsync("departments", ct);
    return Result.Updated;
  }
}