using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Departments.Commands.DeleteDepartment;

public sealed record class DeleteDepartmentCommand (int DepartmentId) : IRequest<Result<Deleted>>;