using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Employees.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Employees;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(
    IUnitOfWork unitWork
) : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
  private readonly IUnitOfWork _unitWork = unitWork;

  public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken ct)
  {
    var person = await _unitWork.Person.GetByIdAsync(request.PersonId, ct);
    if (person is null)
      return ApplicationErrors.PersonNotFound;

    // var existingPatient = await _unitWork.Patients.GetByPersonIdAsync(request.PersonId, ct);
    // if (existingPatient is not null)
    //   return ApplicationErrors.PersonAlreadyAssigned(request.PersonId);

    // var existingDoctor = await _unitWork.Doctors.GetByPersonIdAsync(request.PersonId, ct);
    // if (existingDoctor is not null)
    //   return ApplicationErrors.PersonAlreadyAssigned(request.PersonId);

    // var existingEmployee = await _unitWork.Employees.GetByPersonIdAsync(request.PersonId, ct);
    // if (existingEmployee is not null)
    //   return ApplicationErrors.EmployeeAlreadyExists(request.PersonId);

    var employeeResult = Employee.Create(
        id: request.EmployeeId,
        personId: request.PersonId,
        role: request.Role
    );

    if (employeeResult.IsError)
      return employeeResult.Errors;

    var employee = employeeResult.Value;

    await _unitWork.Employees.AddAsync(employee, ct);
    await _unitWork.SaveChangesAsync(ct);
    return employee.ToDto();
  }
}