using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartmentById;

public sealed class GetDepartmentByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery query, CancellationToken ct)
  {
    var department = await _unitOfWork.Departments.GetByIdAsync(query.DepartmentId, ct);


    if (department is null)
      return ApplicationErrors.DepartmentNotFound;

    return department.ToDto();
  }
}