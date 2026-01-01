using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetEffectiveUserPermissions;

public sealed class GetEffectiveUserPermissionsQueryHandler
    : IRequestHandler<GetEffectiveUserPermissionsQuery, Result<IReadOnlyList<string>>>
{
    private readonly IIdentityService _identityService;

    public GetEffectiveUserPermissionsQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<IReadOnlyList<string>>> Handle(
        GetEffectiveUserPermissionsQuery request,
        CancellationToken ct)
        => _identityService.GetEffectiveUserPermissionsAsync(
            request.UserId,
            ct);
}

