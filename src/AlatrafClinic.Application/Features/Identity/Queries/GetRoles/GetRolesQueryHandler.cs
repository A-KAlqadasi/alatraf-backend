using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetRoles;

public sealed class GetRolesQueryHandler
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDetailsDto>>>
{
    private readonly IIdentityService _identityService;

    public GetRolesQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<IReadOnlyList<RoleDetailsDto>>> Handle(
        GetRolesQuery request,
        CancellationToken ct)
        => _identityService.GetRolesAsync(ct);
}
