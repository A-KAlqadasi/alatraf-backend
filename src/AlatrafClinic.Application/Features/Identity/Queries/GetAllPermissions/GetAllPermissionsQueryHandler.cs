using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler
    : IRequestHandler<GetAllPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    private readonly IIdentityService _identityService;

    public GetAllPermissionsQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<IReadOnlyList<PermissionDto>>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken ct)
        => _identityService.GetAllPermissionsAsync(request.search, ct);
}
