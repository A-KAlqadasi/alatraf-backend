using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeInfo;

public class UpdateEmployeeCommandHandler(
    IPersonUpdateService personUpdateService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateEmployeeCommandHandler> logger
) : IRequestHandler<UpdateEmployeeCommand, Result<Updated>>
{
  private readonly IPersonUpdateService _personUpdateService = personUpdateService;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<UpdateEmployeeCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
  {
    var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId, ct);
    if (employee is null)
    {
      _logger.LogWarning("❌ Employee with ID {EmployeeId} not found.", request.EmployeeId);
      return ApplicationErrors.EmployeeNotFound;
    }

    var person = await _unitOfWork.Person.GetByIdAsync(employee.PersonId, ct);
    if (person is null)
    {
      _logger.LogWarning("❌ Person for Employee {EmployeeId} not found.", request.EmployeeId);
      return ApplicationErrors.PersonNotFound;
    }

    var personUpdate = await _personUpdateService.UpdateAsync(person.Id, request.Person, ct);
    if (personUpdate.IsError)
      return personUpdate.Errors;

    var roleUpdate = employee.UpdateRole(request.Role);
    if (roleUpdate.IsError)
      return roleUpdate.Errors;

    await _unitOfWork.Person.UpdateAsync(person, ct);
    await _unitOfWork.Employees.UpdateAsync(employee, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation("✅ Employee {EmployeeId} and Person {PersonId} updated successfully.", employee.Id, person.Id);

    
    return Result.Updated;
  }
}