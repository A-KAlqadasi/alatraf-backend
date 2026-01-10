using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery(string? search = null)
    : IRequest<Result<IReadOnlyList<PermissionDto>>>;
