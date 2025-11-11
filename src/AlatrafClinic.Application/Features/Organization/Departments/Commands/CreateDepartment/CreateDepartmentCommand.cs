
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands.CreateDepartment;

public sealed record CreateDepartmentCommand(
    string Name
) : IRequest<Result<DepartmentDto>>;
