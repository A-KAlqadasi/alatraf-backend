using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Queries.GetEmployeesByRole;

public sealed record GetEmployeesByRoleQuery(Role? Role = null)
    : IRequest<Result<List<EmployeeDto>>>;
