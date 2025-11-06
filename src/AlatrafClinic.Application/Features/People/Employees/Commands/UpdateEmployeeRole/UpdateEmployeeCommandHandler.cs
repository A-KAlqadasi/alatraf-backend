using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.People.Employees;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeRole;

public class UpdateEmployeeCommandHandler(
    IUnitOfWork unitWork
) : IRequestHandler<UpdateEmployeeCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<Updated>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
  {
    var employee = await _unitWork.Employees.GetByIdAsync(request.EmployeeId, ct);
    if (employee is null)
      return ApplicationErrors.EmployeeNotFound;

        var updateResult = employee.UpdateRole(request.Role);
        if (updateResult.IsError)
            return updateResult.Errors;


    await _unitWork.Employees.UpdateAsync(employee, ct);
    await _unitWork.SaveChangesAsync(ct);

    return Result.Updated;
  }
}