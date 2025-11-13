
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands;

public sealed record RenameDepartmentCommand(
    int DepartmentId,
    string NewName
) : IRequest<Result<Updated>>;
