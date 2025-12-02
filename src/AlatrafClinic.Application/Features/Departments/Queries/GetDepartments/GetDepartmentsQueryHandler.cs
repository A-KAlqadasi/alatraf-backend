using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartments;

public sealed class GetDepartmentsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<DepartmentDto>>> Handle(GetDepartmentsQuery request, CancellationToken ct)
  {
     var departments = await _unitOfWork.Departments.GetAllAsync();


    return departments.ToDtos();
  }
}