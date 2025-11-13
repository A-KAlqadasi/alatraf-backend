using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Application.Features.Organization.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Departments.Queries.GetDepartments;

public sealed class GetDepartmentsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<DepartmentDto>>> Handle(GetDepartmentsQuery request, CancellationToken ct)
  {
    var search = request.SearchTerm?.Trim().ToLower();
    var departments = string.IsNullOrWhiteSpace(search)
        ? await _unitOfWork.Departments.GetAllAsync(ct)
        : await _unitOfWork.Departments.SearchByNameAsync(search, ct);


    return departments.ToDtos();
  }
}