using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Application.Features.Organization.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands;

public sealed class RenameDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<RenameDepartmentCommandHandler> logger,

ICacheService cache
) : IRequestHandler<RenameDepartmentCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<RenameDepartmentCommandHandler> _logger = logger;
    private readonly ICacheService _cache = cache;

    public async Task<Result<Updated>> Handle(RenameDepartmentCommand request, CancellationToken ct)
  {
    var name = request.NewName.Trim();

    var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, ct);
    if (department is null)
    {
      _logger.LogWarning(" Department {DepartmentId} not found.", request.DepartmentId);
      return ApplicationErrors.DepartmentNotFound;
    }

    var existing = await _unitOfWork.Departments.GetByNameAsync(name, ct);
    if (existing is not null && existing.Id != request.DepartmentId)
    {
      _logger.LogWarning("Another department with name '{DepartmentName}' already exists.", name);
      return ApplicationErrors.DepartmentAlreadyExists(name);
    }

    var renameResult = department.Rename(name);
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