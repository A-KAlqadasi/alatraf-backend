using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetEffectiveUserPermissions;

public sealed class GetEffectiveUserPermissionsQueryHandler
    : IRequestHandler<GetEffectiveUserPermissionsQuery, Result<IReadOnlyList<GetEffectivePermissionsDto>>>
{
    private readonly IIdentityService _identityService;

    public GetEffectiveUserPermissionsQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<IReadOnlyList<GetEffectivePermissionsDto>>> Handle(
        GetEffectiveUserPermissionsQuery request,
        CancellationToken ct)
    {
        var result = await _identityService.GetEffectiveUserPermissionsAsync(
            request.UserId,
            ct);

        return result.Value.Select(p => new GetEffectivePermissionsDto
        {
            PermissionName = p
        }).ToList();
    }
    
}

