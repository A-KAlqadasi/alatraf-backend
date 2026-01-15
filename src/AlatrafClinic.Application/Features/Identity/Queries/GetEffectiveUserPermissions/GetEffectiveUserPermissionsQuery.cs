using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetEffectiveUserPermissions;

public sealed record GetEffectiveUserPermissionsQuery(
    string UserId
) : IRequest<Result<IReadOnlyList<GetEffectivePermissionsDto>>>;

