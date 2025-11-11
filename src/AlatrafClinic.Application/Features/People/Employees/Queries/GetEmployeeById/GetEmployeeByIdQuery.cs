using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Queries.GetEmployeeById;

public sealed record GetEmployeeByIdQuery(Guid EmployeeId)
    : IRequest<Result<EmployeeDto>>;
