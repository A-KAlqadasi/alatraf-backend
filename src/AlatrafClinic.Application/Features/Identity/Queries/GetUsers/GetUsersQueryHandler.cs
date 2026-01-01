using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetUsers;

public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<IReadOnlyList<UserListItemDto>>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<IReadOnlyList<UserListItemDto>>> Handle(
        GetUsersQuery request,
        CancellationToken ct)
        => _identityService.GetUsersAsync(ct);
}
