using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartmentById;

public sealed record GetDepartmentByIdQuery(int DepartmentId)
    : IRequest<Result<DepartmentDto>>;
