using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Employees.Mappers;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Employees;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateEmployeeCommandHandler> logger,
    IPersonCreateService personCreateService
) : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<CreateEmployeeCommandHandler> _logger = logger;
  private readonly IPersonCreateService _personCreateService = personCreateService;

  public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken ct)
  {
    var personResult = await _personCreateService.CreateAsync(request.Person, ct);
    if (personResult.IsError)
      return personResult.Errors;

    var person = personResult.Value;

    var employeeResult = Employee.Create(person.Id, request.Role); 
    if (employeeResult.IsError)
      return employeeResult.Errors;

    var employee = employeeResult.Value;

    await _unitOfWork.Person.AddAsync(person, ct);
    await _unitOfWork.Employees.AddAsync(employee, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation("✅ Employee created successfully with ID {EmployeeId}", employee.Id);


    return employee.ToDto();
  }
}