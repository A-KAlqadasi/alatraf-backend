

using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

using MediatR;
namespace AlatrafClinic.Application.Features.People.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    PersonInput Person,
    Role Role
) : IRequest<Result<EmployeeDto>>;

