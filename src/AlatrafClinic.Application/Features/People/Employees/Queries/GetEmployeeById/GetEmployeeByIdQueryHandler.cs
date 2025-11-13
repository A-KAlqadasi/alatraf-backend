using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Employees.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
  {
    var employeeQuery = await _unitOfWork.Employees.GetByIdWithPersonAsync(request.EmployeeId, ct);
if (employeeQuery is null)
      return ApplicationErrors.EmployeeNotFound;
    
    return employeeQuery.ToDto();
  }
}