

using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeRole;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    Role Role
) : IRequest<Result<Updated>>;
