using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeInfo;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    PersonInput Person,
    Role Role
) : IRequest<Result<Updated>>;
