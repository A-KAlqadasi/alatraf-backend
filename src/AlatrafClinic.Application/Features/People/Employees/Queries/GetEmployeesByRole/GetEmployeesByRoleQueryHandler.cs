using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Employees.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.People.Employees.Queries.GetEmployeesByRole;

public class GetEmployeesByRoleQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetEmployeesByRoleQuery, Result<List<EmployeeDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<EmployeeDto>>> Handle(GetEmployeesByRoleQuery request, CancellationToken ct)
  {
    var query = await _unitOfWork.Employees.GetEmployeesWithPersonQueryAsync();

    if (request.Role.HasValue)
      query = query.Where(e => e.Role == request.Role.Value);

    var employees = await query.ToListAsync(ct);

    return employees.ToDtos();
  }
}